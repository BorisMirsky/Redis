using StackExchange.Redis;
using RedisWebApplication;




namespace RedisWebApplication
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redis;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }
        public async Task<string?> GetAsync(string key)
        {
            return await _redis.StringGetAsync(key);
        }
        public async Task SetAsync(string key, string value)
        {
            await _redis.StringSetAsync(key, value, TimeSpan.FromMinutes(30));
        }
    }
}
