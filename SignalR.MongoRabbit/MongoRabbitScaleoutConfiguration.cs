using EasyNetQ;
using RabbitMQ.Client;
using SignalR.RabbitMQ;

namespace SignalR.MongoRabbit
{
    public class MongoRabbitScaleoutConfiguration : RabbitMqScaleoutConfiguration
    {
        public MongoRabbitScaleoutConfiguration(string mongoConnectionString, string ampqConnectionString,
            string exchangeName, string queueName = null, string stampExchangeName = "signalr-stamp")
            : base(ampqConnectionString, exchangeName, queueName, stampExchangeName)
        {
            MongoConnectionString = mongoConnectionString;
        }

        public MongoRabbitScaleoutConfiguration(string mongoConnectionString, ConnectionFactory connectionfactory,
            string exchangeName, string queueName = null, string stampExchangeName = "signalr-stamp")
            : base(connectionfactory, exchangeName, queueName, stampExchangeName)
        {
            MongoConnectionString = mongoConnectionString;
        }

        public MongoRabbitScaleoutConfiguration(string mongoConnectionString, IBus bus, string exchangeName,
            string queueName = null, string stampExchangeName = "signalr-stamp")
            : base(bus, exchangeName, queueName, stampExchangeName)
        {
            MongoConnectionString = mongoConnectionString;
        }

        public string MongoConnectionString { get; set; }
    }
}