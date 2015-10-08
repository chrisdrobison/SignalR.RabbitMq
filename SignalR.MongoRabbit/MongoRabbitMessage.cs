namespace SignalR.MongoRabbit
{
    internal class MongoRabbitMessage
    {
        public ulong Id { get; set; }
        public byte[] Content { get; set; }
    }
}