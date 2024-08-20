using IBB.Nesine.Data;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace IBB.Nesine.Services.Services
{
    public class ParkService : IParkService
    {
        private readonly IDbProvider _dbProvider;

        public ParkService(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public List<GetParksByDistrictResponseModel> GetParksByDistrict(string district)
        {
            return _dbProvider.Query<GetParksByDistrictResponseModel>("[dbo].[usp_GetParkByDistrict]", new { District = district }).ToList();
        }
    }
}
