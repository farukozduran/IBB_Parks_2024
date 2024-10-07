using IBB.Nesine.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Interfaces
{
    public interface IParkService
    {
        IEnumerable<GetParksByDistrictResponseModel> GetParksByDistrict(string district);
        bool GetParkAvailabilityByParkId(int parkId);
        Task<bool> UpdateParksInfoAsync();
        IEnumerable<Park> GetParkList();
    }
}
