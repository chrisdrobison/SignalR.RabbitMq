using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SignalR.MongoRabbit
{
    internal class MongoRabbitCount
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public ulong Count { get; set; }
    }
}