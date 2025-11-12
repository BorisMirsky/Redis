using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RedisWebApplication;
using System.Text.Json;



var builder = WebApplication.CreateBuilder(args);

string hostname = "'redis-15653.c246.us-east-1-4.ec2.redns.redis-cloud.com";
string password = "ewkA5yExjutxSRFLmYhJODJ9HjXBSU6F";


string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connection));
builder.Services.AddTransient<UserService>();
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = hostname; // "localhost";
    options.InstanceName = "local";
});
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
//app.MapGet("/user/{id}", async (int id, UserService userService) =>
//{
//    User? user = await userService.GetUser(id);
//    if (user != null) return $"User {user.Name}  Id={user.Id}  Age={user.Age}";
//    return "User not found";
//});
//app.MapGet("/", () => "Hello World!");

app.Run();
