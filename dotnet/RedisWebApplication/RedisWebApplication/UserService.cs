using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;



namespace RedisWebApplication
{
    public class UserService
    {
        
        ApplicationContext db;
        IDistributedCache cache1;
        IDatabase cache2;          //private readonly 

        public UserService(ApplicationContext context,
            IDistributedCache distributedCache, IDatabase cache)
        {
            db = context;
            cache1 = distributedCache;
            cache2 = cache;
        }

        public async Task<User?> GetUser(int id)
        {
            User? user = null;
            // пытаемся получить данные из кэша по id
            string? userString = await cache2.GetStringAsync(id.ToString());
            //десериализируем из строки в объект User
            if (userString != null) user = JsonSerializer.Deserialize<User>(userString);
            // если данные не найдены в кэше
            if (user == null)
            {
                // обращаемся к базе данных
                user = await db.Users.FindAsync(id);
                // если пользователь найден, то добавляем в кэш
                if (user != null)
                {
                    Debug.WriteLine($"{user.Name} извлечен из базы данных");
                    // сериализуем данные в строку в формате json
                    userString = JsonSerializer.Serialize(user);
                    // сохраняем строковое представление объекта в формате json в кэш на 2 минуты
                    await cache.SetStringAsync(user.Id.ToString(), userString, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                    });
                }
            }
            else
            {
                Debug.WriteLine($"{user.Name} извлечен из кэша");
            }
            return user;
        }

    }
}
