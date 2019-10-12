namespace ConsoleApp.Domain
{
    public sealed class ShardEnvelope
    {
        public readonly int ShardId;
        public readonly int EntityId;
        public readonly object Message;
        public ShardEnvelope(int shardId, int entityId, object message)
        {
            this.ShardId = shardId;
            this.EntityId = entityId;
            this.Message = message;
        }
    }
}
