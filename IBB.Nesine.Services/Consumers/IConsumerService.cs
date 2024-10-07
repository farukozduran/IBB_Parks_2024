namespace IBB.Nesine.Services.Consumers
{
    public interface IConsumerService
    {
        void Consume(string queueName);
    }
}