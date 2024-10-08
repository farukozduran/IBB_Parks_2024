﻿using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace IBB.Nesine.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParksController : Controller
    {
        private readonly IParkService _parkService;

        public ParksController(IParkService parkService)
        {
            _parkService = parkService;
        }

        [HttpPost("UpdateParksInfo")]
        public async Task<bool> UpdateParksInfoAsync()
        {
            return await _parkService.UpdateParksInfoAsync();
        }

        [HttpGet("GetParkByDistrict")]
        public ActionResult<List<GetParksByDistrictResponseModel>> GetParkByDistrict(string district)
        {
            return Ok(_parkService.GetParksByDistrict(district));
        }
        [Authorize]
        [HttpGet("GetParkAvailabilityByParkId")]
        public ActionResult<bool> GetParkAvailabilityByParkId(int parkId)
        {
            return Ok(_parkService.GetParkAvailabilityByParkId(parkId));
        }

        [HttpGet("GetAllParks")]
        public ActionResult<List<Park>> GetAllParks()
        {
            return Ok(_parkService.GetParkList());
        }
    }
}
