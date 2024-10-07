using IBB.Nesine.Caching.Providers;
using IBB.Nesine.Data;
using IBB.Nesine.Services.Consumers;
using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Models;
using IBB.Nesine.Services.Producers;
using IBB.Nesine.Services.Queue;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Services
{
    public class ParkService : IParkService
    {
        private readonly IDbProvider _dbProvider;
        private readonly ApiServiceHelper _apiServiceHelper;
        private readonly string _parkListUrl;
        private readonly string _updateParksQueue;
        private readonly ILogger<ParkService> _logger;
        private readonly RedisHelper _redisHelper;
        private readonly RabbitMqProducer _rabbitMqProducer;
        private readonly RabbitMqConsumer _rabbitMqConsumer;
        private readonly UpdateParksInfoConsumer _updateParksInfoConsumer;

        public ParkService(IDbProvider dbProvider
            , ApiServiceHelper apiServiceHelper
            , IConfiguration configuration
            , ILogger<ParkService> logger
            , RedisHelper redisHelper
            , RabbitMqProducer rabbitMqProducer
            , RabbitMqConsumer rabbitMqConsumer
            , UpdateParksInfoConsumer updateParksInfoConsumer)
        {
            _dbProvider = dbProvider;
            _apiServiceHelper = apiServiceHelper;
            _parkListUrl = configuration.GetSection("ParkApiUrl:ParkList").Value;
            _updateParksQueue = configuration.GetSection("RabbitMqQueueSettings:UpdateParksQueue:QueueName").Value;
            _logger = logger;
            _redisHelper = redisHelper;
            _rabbitMqProducer = rabbitMqProducer;
            _rabbitMqConsumer = rabbitMqConsumer;
            _updateParksInfoConsumer = updateParksInfoConsumer;
        }
        public IEnumerable<GetParksByDistrictResponseModel> GetParksByDistrict(string district)
        {
            string _cacheKey = $"parksByDistrict_{district}", spName = "[dbo].[usp_GetParkByDistrict]";
            return _redisHelper.GetData<GetParksByDistrictResponseModel>(_cacheKey, TimeSpan.FromMinutes(30), spName, new { District = district });
        }
        public bool GetParkAvailabilityByParkId(int parkId)
        {
            string _cacheKey = $"parkAvailabilityByParkId_{parkId}", spName = "usp_GetParkAvailabilityById";
            var availabilityModels = _redisHelper.GetData<GetParkAvailabilityModel>(_cacheKey, TimeSpan.FromMinutes(30), spName, new { ParkId = parkId });
            return availabilityModels.FirstOrDefault().IsAvailable;
        }
        public async Task<bool> UpdateParksInfoAsync()
        {
            List<Park> data = await _apiServiceHelper.GetAsync<List<Park>>(_parkListUrl);

            if (!data.Any()) return false;

            int batchSize = 50;

            for (int i = 0; i < data.Count; i += batchSize)
            {
                List<Park> parkBatch = data.GetRange(i, Math.Min(batchSize, data.Count - i));
                DataTable parksDataTable = DataTableHelper.ToDataTable(parkBatch);
                var message = JsonConvert.SerializeObject(parksDataTable);
                _logger.LogDebug("update parks before queue " + _updateParksQueue);
                _rabbitMqProducer.Produce(_updateParksQueue, message);
                _logger.LogDebug("update parks after publish queue " + _updateParksQueue);
            }
            return true;
        }
        public IEnumerable<Park> GetParkList()
        {
            string _cacheKey = "parkList", spName = "[dbo].[usp_GetAllParks]";
            return _redisHelper.GetData<Park>(_cacheKey, TimeSpan.FromMinutes(30), spName, null);
        }
    }
}
