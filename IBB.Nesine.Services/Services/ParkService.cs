using IBB.Nesine.Caching;
using IBB.Nesine.Data;
using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Services
{
    public class ParkService : IParkService
    {
        public string GetListKey { get; set; }
        private readonly IDbProvider _dbProvider;
        private readonly ApiServiceHelper _apiServiceHelper;
        private readonly string _parkListUrl;
        private readonly ILogger<ParkService> _logger;
        private readonly ICacheProvider _cacheProvider;

        public ParkService(IDbProvider dbProvider, ApiServiceHelper apiServiceHelper, IConfiguration configuration, ILogger<ParkService> logger,ICacheProvider cacheProvider)
        {
            _dbProvider = dbProvider;
            _apiServiceHelper = apiServiceHelper;
            _parkListUrl = configuration.GetSection("ParkApiUrl:ParkList").Value;
            _logger = logger;
            _cacheProvider = cacheProvider;
        }
        public List<GetParksByDistrictResponseModel> GetParksByDistrict(string district)
        {
            return _dbProvider.Query<GetParksByDistrictResponseModel>("[dbo].[usp_GetParkByDistrict]", new { District = district }).ToList();
        }
        public bool GetParkAvailabilityByParkId(int parkId)
        {
            return _dbProvider.QuerySingle<GetParkAvailabilityModel>("usp_GetParkAvailabilityById", new { ParkId = parkId }).IsAvailable;
        }
        public async Task<bool> UpdateParksInfoAsync()
        {
            GetListKey = "$GetParkList";

            List<Park> data = await _apiServiceHelper.GetAsync<List<Park>>(_parkListUrl);

            _cacheProvider.Set<List<Park>>(GetListKey, data, TimeSpan.FromSeconds(6000));

            int batchSize = 50;

            if (!data.Any()) return false;

            for (int i = 0; i < data.Count; i += batchSize)
            {
                _logger.LogDebug("update parks before sp");

                List<Park> parkBatch = data.GetRange(i, Math.Min(batchSize, data.Count - i));

                DataTable parksDataTable = DataTableHelper.ToDataTable(parkBatch);

                _dbProvider.Execute("usp_BulkInsertOrUpdateParks", new { parksTable = parksDataTable });

                _logger.LogDebug("after parks before sp");
            }

            _cacheProvider.Get<List<Park>>(GetListKey);

            return true;
        }
    }
}
