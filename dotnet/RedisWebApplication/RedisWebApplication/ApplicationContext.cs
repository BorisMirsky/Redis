
using Microsoft.EntityFrameworkCore;


namespace RedisWebApplication
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> User { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) 
            {}
    }
}
