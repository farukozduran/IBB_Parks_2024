using IBB.Nesine.Data;
using IBB.Nesine.Services.Interfaces;
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

        public async Task<bool> GetParkAvailability(int parkId)
        {
            var isAvailable = _dbProvider.Execute("usp_GetParkAvailabilityById", new {ParkId = parkId});
        }
    }
}
