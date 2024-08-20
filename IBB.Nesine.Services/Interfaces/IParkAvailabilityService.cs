using System.Threading.Tasks;

namespace IBB.Nesine.Services.Interfaces
{
    public interface IParkAvailabilityService
    {
        bool GetParkAvailability(int parkId);
    }
}
