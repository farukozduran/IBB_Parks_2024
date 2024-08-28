using IBB.Nesine.Data;
using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Services
{
    public class ParkService : IParkService
    {
        private readonly IDbProvider _dbProvider;
        private readonly ApiServiceHelper _apiServiceHelper;
        private readonly string _parkListUrl;


        public ParkService(IDbProvider dbProvider, ApiServiceHelper apiServiceHelper, IConfiguration configuration)
        {
            _dbProvider = dbProvider;
            _apiServiceHelper = apiServiceHelper;
            _parkListUrl = configuration.GetSection("ParkApiUrl:ParkList").Value;
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
            List<Park> data = await _apiServiceHelper.GetAsync<List<Park>>(_parkListUrl);
            int batchSize = 50;
            if (!data.Any()) return false;

            for (int i = 0; i < data.Count; i += batchSize)
            {
                List<Park> parkBatch = data.GetRange(i, Math.Min(batchSize, data.Count - i));
                DataTable parksDataTable = DataTableHelper.ToDataTable(parkBatch);
                _dbProvider.Execute("usp_BulkInsertOrUpdateParks", new { parksTable = parksDataTable });
            }
            return true;
        }
    }
}
