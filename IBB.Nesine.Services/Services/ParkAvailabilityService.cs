using IBB.Nesine.Data;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Services
{
    public class ParkAvailabilityService : IParkAvailabilityService
    {
        private readonly IDbProvider _dbProvider;

        public ParkAvailabilityService(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public bool GetParkAvailability(int parkId)
        {
            var isAvailable = _dbProvider.QuerySingle<GetParkAvailabilityModel>("usp_GetParkAvailabilityById", new { ParkId = parkId });

            return isAvailable.IsAvailable == 0 ? false : true;
        }
    }
}
