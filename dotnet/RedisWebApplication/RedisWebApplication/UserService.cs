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
