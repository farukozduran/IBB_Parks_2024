using IBB.Nesine.Data;
using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Consumers
{
    public class UpdateParksInfoConsumer : RabbitMqConsumer
    {
        private readonly RabbitMqConsumer _rabbitMqConsumer;
        private readonly IDbProvider _dbProvider;
        private readonly string _updateParksQueue;

        public UpdateParksInfoConsumer(RabbitMqConsumer rabbitMqConsumer
            , IDbProvider dbProvider
            , IConfiguration configuration)
        {
            _rabbitMqConsumer = rabbitMqConsumer;
            _dbProvider = dbProvider;
            _updateParksQueue = configuration.GetSection("RabbitMqQueueSettings:UpdateParksQueue:QueueName").Value;
            _rabbitMqConsumer.Consume<List<Park>>(_updateParksQueue, ProcessParkUpdate);
        }

        public async Task ProcessParkUpdate(List<Park> parks)
        {
            try
            {
                //var parks = JsonSerializer.Deserialize<List<Park>>(message);
                DataTable parksDataTable = DataTableHelper.ToDataTable(parks);
                _dbProvider.Execute("usp_BulkInsertOrUpdateParks", new { parksTable = parksDataTable });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
