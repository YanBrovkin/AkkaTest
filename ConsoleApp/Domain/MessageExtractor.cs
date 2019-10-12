using Akka.Cluster.Sharding;

namespace ConsoleApp.Domain
{
    public sealed class MessageExtractor : IMessageExtractor
    {
        public string EntityId(object message) 
            => (message as ShardEnvelope)?.EntityId.ToString();
        public string ShardId(object message) 
            => (message as ShardEnvelope)?.ShardId.ToString();
        public object EntityMessage(object message) 
            => (message as ShardEnvelope)?.Message;
    }
}
