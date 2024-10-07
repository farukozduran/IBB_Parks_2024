using IBB.Nesine.Data;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IDbProvider _dbProvider;
        public UserService(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }
        public async Task<string> Register(UserModel user)
        {
            string checkUserSql = "SELECT COUNT(1) FROM Users WHERE UserName = @UserName";
            var userExists =  _dbProvider.ExecuteScalarAsync<bool>(checkUserSql, new { user.UserName });

            if (userExists)
            {
                return "This username has been taken";
            }
            else
            {
                var sql = "INSERT INTO Users (UserName, Password) VALUES (@UserName, @Password);";
                _dbProvider.ExecuteSql(sql, new { user.UserName, user.Password });
                return "User created successfully";
            }
        }
        public bool CheckLoginInfo(UserModel user)
        {
            string sql = "SELECT Password FROM Users WHERE UserName = @UserName";
            var password = _dbProvider.ExecuteScalarAsync<string>(sql, new { user.UserName });

            if ((password != user.Password))
            {
                return false;
            }
            return true;
        }
    }
}
