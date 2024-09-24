using IBB.Nesine.Services.Models;

namespace IBB.Nesine.Services.Interfaces
{
    public interface IAuthService
    {
        public bool CheckLoginInfo(UserModel user);
        public string GetJwtToken(UserModel user);

    }
}
