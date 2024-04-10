using StackExchange.Redis;

namespace ChatApplication2.Hubs
{
    public class RedisConnection
    {
        private readonly IDatabase _redis;

        public RedisConnection(ConnectionMultiplexer redisConnection) 
        { 
            _redis=redisConnection.GetDatabase();
        }
        public async Task AddConnectionAsync(string userId, string connectionId)
        {
            await _redis.StringSetAsync(userId, connectionId);
        }
        public async Task<string> GetConnIdAsync(string userId)
        {
            return await _redis.StringGetAsync(userId);
        }
    }

}
