using IBB.Nesine.Data;
using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using Microsoft.Extensions.Configuration;
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
        private readonly string _apiUrlParkDetail;
        private readonly string _apiUrlParkList;


        public ParkService(IDbProvider dbProvider, ApiServiceHelper apiServiceHelper, IConfiguration configuration)
        {
            _dbProvider = dbProvider;
            _apiServiceHelper = apiServiceHelper;
            _apiUrlParkDetail = configuration.GetSection("ParkService:ApiUrlParkDetail").Value;
            _apiUrlParkList = configuration.GetSection("ParkService:ApiUrlParkList").Value;
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
                    url = $"{_apiUrlParkDetail}{parkId}";
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

        public async Task<bool> UpdateParksInfoAsync()
        {
            List<Park> data = await _apiServiceHelper.GetAsync<List<Park>>(_apiUrlParkList);

            if (!data.Any()) return false;

            var existingParkIds = _dbProvider.Query<int>("usp_SelectParksByParkId").ToList();

            var parksToInsert = data.Where(p => !existingParkIds.Contains(p.ParkId)).ToList();

            if (parksToInsert.Any())
            {
                _dbProvider.BulkInsert(parksToInsert);
            }
            else
            {
                return false; // eğer aynı parkId'de park varsa update date eklemesi gerekiyor.
            }

            return true;
        }
    }
}
