using System.Threading.Tasks;

namespace IBB.Nesine.Services.Interfaces
{
    public interface IIBBService
    {
        Task<bool> UpdateIBBParksInfoAsync();
    }
}
