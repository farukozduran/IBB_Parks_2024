using IBB.Nesine.Data;
using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Services
{
    public class ParkService : IParkService, IJob
    {
        private readonly IDbProvider _dbProvider;
        private readonly ApiServiceHelper _apiServiceHelper;
        private readonly string apiUrl = "https://api.ibb.gov.tr/ispark/ParkDetay?id=";

        public ParkService(IDbProvider dbProvider, ApiServiceHelper apiServiceHelper)
        {
            _dbProvider = dbProvider;
            _apiServiceHelper = apiServiceHelper;
        }
        public List<GetParksByDistrictResponseModel> GetParksByDistrict(string district)
        {
            return _dbProvider.Query<GetParksByDistrictResponseModel>("[dbo].[usp_GetParkByDistrict]", new { District = district }).ToList();
        }
        public bool GetParkAvailabilityByParkId(int parkId)
        {
            return _dbProvider.QuerySingle<GetParkAvailabilityModel>("usp_GetParkAvailabilityById", new { ParkId = parkId }).IsAvailable;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var parkIds = _dbProvider.Query<int>("usp_SelectParksByParkId");

            try
            {
                string url = string.Empty;
                foreach (int parkId in parkIds)
                {
                    url = $"{apiUrl}{parkId}";
                    var data = await _apiServiceHelper.GetAsync<List<EmptyCapacityResponseModel>>(url);
                    _dbProvider.Execute("usp_SetIsAvailable", new { IsAvailable = data.First().EmptyCapacity > 0, ParkId = parkId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
