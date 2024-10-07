using RabbitMQ.Client;
using System;
using System.Text;

namespace IBB.Nesine.Services.Producers
{
    public class RabbitMqProducer
    {
        private readonly IConnection _connection;

        public RabbitMqProducer()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
        }

        public void Produce(string queueName, string message)
        {
            using var _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            Console.WriteLine($"Veri kuyruğa gönderildi: {message}");
        }
    }
}
