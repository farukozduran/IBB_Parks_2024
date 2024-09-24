using IBB.Nesine.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Interfaces
{
    public interface IUserService
    {
        public Task<string> AddUser(UserModel user);

    }
}
