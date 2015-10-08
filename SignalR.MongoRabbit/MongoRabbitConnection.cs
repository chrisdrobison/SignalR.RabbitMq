using System;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.Topology;
using MongoDB.Bson;
using MongoDB.Driver;
using SignalR.RabbitMQ;

namespace SignalR.MongoRabbit
{
    public class MongoRabbitConnection : RabbitConnectionBase
    {
        private const string CollectionName = "SignalR.MongoRabbit";

        private readonly IAdvancedBus _bus;
        private readonly IMongoCollection<MongoRabbitCount> _collection;
        private IExchange _receiveExchange;
        private IQueue _queue;

        public MongoRabbitConnection(MongoRabbitScaleoutConfiguration configuration) : base(configuration)
        {
            var mongoUrl = new MongoUrl(configuration.MongoConnectionString);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _collection = database.GetCollection<MongoRabbitCount>(CollectionName);

            _bus = configuration.Bus != null
                ? configuration.Bus.Advanced
                : RabbitHutch.CreateBus(configuration.AmpqConnectionString).Advanced;

            //wire up the reconnection handler
            _bus.Connected += OnReconnection;

            //wire up the disconnection handler
            _bus.Disconnected += OnDisconnection;
        }

        public override void Send(RabbitMqMessageWrapper message)
        {
            var mongoRabbitMessage = new MongoRabbitMessage()
            {
                Id = GetNewMessageId(),
                Content = message.Bytes
            };
            var rabbitMessage = new Message<MongoRabbitMessage>(mongoRabbitMessage);
            _bus.Publish(_receiveExchange, String.Empty, false, false, rabbitMessage);
        }

        public override void StartListening()
        {
            _receiveExchange = _bus.ExchangeDeclare(Configuration.ExchangeName, ExchangeType.Fanout);
            _queue = Configuration.QueueName == null
                        ? _bus.QueueDeclare()
                        : _bus.QueueDeclare(Configuration.QueueName);

            _bus.Bind(_receiveExchange, _queue, "#");
            _bus.Consume<MongoRabbitMessage>(_queue, (message, info) =>
            {
                var wrapperMsg = new RabbitMqMessageWrapper()
                {
                    Id = message.Body.Id,
                    Bytes = message.Body.Content
                };
                return Task.Factory.StartNew(() => OnMessage(wrapperMsg));
            });
        }

        private ulong GetNewMessageId()
        {
            var filter = new BsonDocument();
            var update = Builders<MongoRabbitCount>.Update.Inc(count => count.Count, (ulong)1);
            var result = _collection.FindOneAndUpdateAsync(filter, update,
                new FindOneAndUpdateOptions<MongoRabbitCount, MongoRabbitCount>()
                {
                    IsUpsert = true,
                    ReturnDocument = ReturnDocument.After
                }).GetAwaiter().GetResult();
            return result.Count;
        }
    }
}