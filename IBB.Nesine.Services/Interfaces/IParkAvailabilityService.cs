using System.Threading.Tasks;

namespace IBB.Nesine.Services.Interfaces
{
    public interface IParkAvailabilityService
    {
        Task<bool> GetParkAvailability(int parkId);
    }
}
