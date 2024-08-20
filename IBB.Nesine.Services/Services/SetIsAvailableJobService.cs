using IBB.Nesine.Data;
using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using Newtonsoft.Json;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Services
{
    public class SetIsAvailableJobService : IJob
    {
        private readonly ApiServiceHelper _apiServiceHelper;
        private readonly IDbProvider _dbProvider;
        private readonly string apiUrl = "https://api.ibb.gov.tr/ispark/ParkDetay?id=";
        public int isAvailable;

        public SetIsAvailableJobService(ApiServiceHelper apiServiceHelper, IDbProvider dbProvider)
        {
            _apiServiceHelper = apiServiceHelper;
            _dbProvider = dbProvider;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var parkIds = _dbProvider.Query<int>("usp_SelectParksByParkId");

            try
            {
                foreach (int parkId in parkIds)
                {
                    
                    string url = $"{apiUrl}{parkId}";
                    var data = await _apiServiceHelper.GetAsync<List<EmptyCapacityResponseModel>>(url);

                    foreach (EmptyCapacityResponseModel park in data)
                    {

                        isAvailable = park.EmptyCapacity > 0 ? 1 : 0;

                        _dbProvider.Execute("usp_SetIsAvailable",
                                        new { IsAvailable = isAvailable, ParkId = parkId });
                    }
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
