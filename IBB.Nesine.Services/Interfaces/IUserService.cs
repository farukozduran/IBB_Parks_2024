using IBB.Nesine.Services.Models;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Interfaces
{
    public interface IUserService
    {
        public Task<string> Register(UserModel user);
        public bool CheckLoginInfo(UserModel user);

    }
}
