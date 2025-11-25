using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;



namespace RedisWebApplication
{
    public class UserService
    {
        
        ApplicationContext db;
        IDistributedCache cache;

        public UserService(ApplicationContext context,
            IDistributedCache distributedCache)
        {
            db = context;
            cache = distributedCache;
        }

        public async Task<User?> GetUser(int id)
        {
            User? user = null;
            // пытаемся получить данные из кэша по id
            var userString = await cache.GetStringAsync(id.ToString());
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
