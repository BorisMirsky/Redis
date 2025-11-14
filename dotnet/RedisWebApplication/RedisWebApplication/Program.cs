using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RedisWebApplication;
using StackExchange.Redis;
using System.Text.Json;



var builder = WebApplication.CreateBuilder(args);

string hostname = "redis-15653.c246.us-east-1-4.ec2.redns.redis-cloud.com";
string password = "ewkA5yExjutxSRFLmYhJODJ9HjXBSU6F";
const string endpoint = "redis-15653.c246.us-east-1-4.ec2.redns.redis-cloud.com:15653,password=ewkA5yExjutxSRFLmYhJODJ9HjXBSU6F";

string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;


builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connection));
//builder.Services.AddControllers();
builder.Services.AddTransient<UserService>();
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = hostname;
    //options.InstanceName = password;   // "local";
});


//      ?!?!?!
builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
  ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString(endpoint)));


builder.Services.AddSwaggerGen();
//builder.Services.AddOpenApi();

var app = builder.Build();


app.MapGet("/", () => "Hello ASP.NET Core Cache!");
app.MapGet("/user/{id}", (int id, [FromServices] UserService data) =>
{
    var _user = data.GetUser(id);
    if (_user == null)
    {
        return Results.Content($"Пользователь не найден");
    }
    return Results.Json(_user);
});


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
