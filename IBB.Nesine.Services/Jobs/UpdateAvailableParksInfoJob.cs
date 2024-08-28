using IBB.Nesine.Data;
using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Models;
using Microsoft.Extensions.Configuration;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Jobs
{
    public class UpdateAvailableParksInfoJob : IJob
    {
        private readonly IDbProvider _dbProvider;
        private readonly ApiServiceHelper _apiServiceHelper;
        private readonly string _parkDetailUrl;

        public UpdateAvailableParksInfoJob(IDbProvider dbProvider, ApiServiceHelper apiServiceHelper, IConfiguration configuration)
        {
            _dbProvider = dbProvider;
            _apiServiceHelper = apiServiceHelper;
            _parkDetailUrl = configuration.GetSection("ParkApiUrl:ParkDetail").Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var parkIds = _dbProvider.Query<int>("usp_SelectParksByParkId");
            int batchSize = 50;
            List<UpdateAvailableInfoModel> updateList = new();
            try
            {
                string url = string.Empty;
                foreach (int parkId in parkIds)
                {
                    Console.WriteLine(parkId);
                    url = $"{_parkDetailUrl}{parkId}";
                    var data = await _apiServiceHelper.GetAsync<List<EmptyCapacityResponseModel>>(url);
                    updateList.Add(new UpdateAvailableInfoModel { IsAvailable = data.First().EmptyCapacity > 0, ParkId = parkId });
                    if (updateList.Count == batchSize)
                    {
                        DataTable dt = DataTableHelper.ToDataTable(updateList);
                        _dbProvider.Execute("usp_SetIsAvailable", new { updateAvailableInfoTable = dt });
                        updateList.Clear();
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
