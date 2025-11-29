using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;




namespace RedisWebApplication
{
    public class UserService
    {
        private readonly ApplicationContext db;
        private readonly IDatabase _redis;
        private readonly IDistributedCache cache;

        public UserService(
            ApplicationContext context, 
            IConnectionMultiplexer redis,
            IDistributedCache distributedCache)
        {
            db = context;
            _redis = redis.GetDatabase();
            cache = distributedCache;
        }

        public async Task<User?> GetUser(int id)
        {
            User? user = null;
            // есть данные в кэше?
             string? userString = await _redis.StringGetAsync(id.ToString());
            // десериализируем из строки в объект User
            if (userString != null) user = JsonSerializer.Deserialize<User>(userString);
            // нет данных в кэше
            if (user == null)
            {
                // обращаемся к базе данных
                user = await db.User.FindAsync(id);
                // если пользователь найден, то добавляем в кэш
                if (user != null)
                {
                    Debug.WriteLine($"{user.Name} извлечен из базы данных");
                    // сериализуем данные в строку в формате json
                    userString = JsonSerializer.Serialize(user);
                    // сохраняем строковое представление объекта в формате json в кэш на 2 минуты
                    await _redis.StringSetAsync(user.Id.ToString(), userString, TimeSpan.FromMinutes(2));
                }
            }
            // нашли в кеше и извлекли оттуда
            else
            {
                Debug.WriteLine($"{user.Name} извлечен из кэша");
            }
            return user;
        }
    }
}
