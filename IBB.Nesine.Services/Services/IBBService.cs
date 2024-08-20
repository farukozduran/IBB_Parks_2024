using IBB.Nesine.Data;
using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Services
{
    public class IBBService : IIBBService
    {
        private readonly ApiServiceHelper _apiServiceHelper;
        private readonly IDbProvider _dbProvider;

        public IBBService(ApiServiceHelper apiServiceHelper, IDbProvider dbProvider)
        {
            _apiServiceHelper = apiServiceHelper;
            _dbProvider = dbProvider;
        }

        public async Task<bool> UpdateIBBParksInfoAsync()
        {
            string url = "https://api.ibb.gov.tr/ispark/Park";
            List<Park> data = await _apiServiceHelper.GetAsync<List<Park>>(url);

            if (!data.Any()) return false;

            try
            {
                foreach (var park in data)
                {
                    _dbProvider.Execute("usp_AddPark", new
                    {
                        park.ParkId,
                        park.ParkName,
                        park.Lat,
                        park.Lng,
                        park.Capacity,
                        park.EmptyCapacity,
                        park.WorkHours,
                        park.ParkType,
                        park.FreeTime,
                        park.District,
                        park.IsOpen
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

            return true;
        }

    }
}
