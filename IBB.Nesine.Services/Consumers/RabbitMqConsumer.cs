using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace IBB.Nesine.Services.Consumers
{
    public class RabbitMqConsumer
    {
        private IConnection _connection;
        private readonly string _hostName = "localhost";
        private IModel _channel;

        public RabbitMqConsumer()
        {
            var factory = new ConnectionFactory() { HostName = _hostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void Consume<T>(string queueName, Func<T, Task> processMessage)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                try
                {

                    var data = JsonConvert.DeserializeObject<T>(message);
                    await processMessage(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            Console.WriteLine($"[*] Waiting for messages in queue: {queueName}");
        }
    }
}
