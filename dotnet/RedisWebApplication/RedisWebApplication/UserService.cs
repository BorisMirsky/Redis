using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;




namespace RedisWebApplication
{
    public class UserService
    {
        
        ApplicationContext db;
        IDistributedCache cache;
        ILogger _logger;

        public UserService(ApplicationContext context,
            ILogger<UserService> logger, 
            IDistributedCache distributedCache)
        {
            db = context;
            _logger = logger;
            cache = distributedCache;
        }
        public async Task<User?> GetUser1(int id)
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
                    Console.WriteLine($"{user.Name} извлечен из базы данных");
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
                Console.WriteLine($"{user.Name} извлечен из кэша");
            }
            return user;
        }


        public User GetUser(int id)
        {
            User user = null;
            var userJson = cache.GetString(id.ToString());
            if (string.IsNullOrEmpty(userJson) == false)
            {
                user = JsonSerializer.Deserialize<User>(userJson);
            }

            if (user == null)  //не смогли получить данные из кэша
            {
                user = db.Users.FirstOrDefault(x => x.Id == id);
                if (user != null) //данные в БД есть
                {
                    cache.SetString(id.ToString(), JsonSerializer.Serialize<User>(user),
                        new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(30)) //через 30 секунд элемент будет удален
                        .SetSlidingExpiration(TimeSpan.FromSeconds(10)) //если в течение 10 секунд к объекту не будет обращения - он будет удален
                        );
                    _logger.LogInformation($"Пользователь {user.Name} помещен в кэш");
                }
            }
            else
            {
                _logger.LogInformation($"Пользователь {user.Name} был извлечен из кэша");
            }
            return user;
        }
    }
}
