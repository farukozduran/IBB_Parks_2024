using IBB.Nesine.Data;
using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Models;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Consumers
{
    public class UpdateAvailableParksInfoJobConsumer : RabbitMqConsumer
    {
        public RabbitMqConsumer _rabbitMqConsumer;
        public IDbProvider _dbProvider;
        public string _updateAvailableParksInfoQueue;

        public UpdateAvailableParksInfoJobConsumer(
            RabbitMqConsumer rabbitMqConsumer
            , IDbProvider dbProvider
            , IConfiguration configuration)
        {
            _rabbitMqConsumer = rabbitMqConsumer;
            _dbProvider = dbProvider;
            _updateAvailableParksInfoQueue = configuration.GetSection("RabbitMqQueueSettings:UpdateAvailableParksInfoJob:QueueName").Value;
            _rabbitMqConsumer.Consume<List<UpdateAvailableInfoModel>>(_updateAvailableParksInfoQueue, ProcessAvailableParkJob);
        }
        public async Task ProcessAvailableParkJob(List<UpdateAvailableInfoModel> parks)
        {
            try
            {
                //var parks = JsonSerializer.Deserialize<List<UpdateAvailableInfoModel>>(message);
                DataTable parksAvailableDataTable = DataTableHelper.ToDataTable(parks);
                _dbProvider.Execute("usp_SetIsAvailable", new { updateAvailableInfoTable = parksAvailableDataTable });
                Console.WriteLine("Veritabanına başarılı bir şekilde güncelleme gönderildi.");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}