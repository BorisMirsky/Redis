using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RedisWebApplication;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;
using NRedisStack;
using NRedisStack.RedisStackCommands;



var builder = WebApplication.CreateBuilder(args);
//15653
string hostname = "redis-15653.c246.us-east-1-4.ec2.redns.redis-cloud.com";
string password = "ewkA5yExjutxSRFLmYhJODJ9HjXBSU6F";
//const string endpoint = "redis-15653.c246.us-east-1-4.ec2.redns.redis-cloud.com:15653,password=ewkA5yExjutxSRFLmYhJODJ9HjXBSU6F";
ConfigurationOptions conf = new ConfigurationOptions
{
    EndPoints = { hostname },
    User = "default",  //"#2678700",
    Password = password
};
ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(conf);
IDatabase db = redis.GetDatabase();
db.StringSet("foo", "bar");
Debug.WriteLine(db.StringGet("foo"));



string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connection));
//builder.Services.AddControllers();
builder.Services.AddTransient<UserService>();
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = hostname;
    //options.InstanceName = password;   // "local";
});

//string connectionString = "your-redis-cloud-host.redis.cloud:12345,password=your-redis-cloud-password";
//ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(endpoint);
//IDatabase db_redis = redis.GetDatabase();


//      ?!?!?!
//builder.Services.AddScoped<IConnectionMultiplexer>(opt =>
//  ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString(endpoint)));


builder.Services.AddSwaggerGen();
//builder.Services.AddOpenApi();
var app = builder.Build();
//app.MapGet("/", () => "Hello ASP.NET Core Cache!");
//app.MapGet("/user/{id}", (int id, [FromServices] UserService data) =>
//{
//    var _user = data.GetUser(id);
//    if (_user == null)
//    {
//        return Results.Content($"Пользователь не найден");
//    }
//    return Results.Json(_user);
//});

if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
//.UseAuthorization();
//app.MapControllers();
app.Run();
