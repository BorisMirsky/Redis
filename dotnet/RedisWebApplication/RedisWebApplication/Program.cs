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
string host = CredentialsSettings.hostname;   
string pass = CredentialsSettings.password;  

//ConfigurationOptions conf = new ConfigurationOptions
//{
//    EndPoints = { host },
//    User = "default",  //"#2678700",
//    Password = pass
//};

string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connection));
builder.Services.AddTransient<UserService>();
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost";   //host;
    options.InstanceName = "local";   // pass;   // "local";
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddSwaggerGen();



var app = builder.Build();
app.MapGet("/", () => "Hello ASP.NET Core Cache!");
app.MapGet("/user/{id}", (int id, [FromServices] UserService data) =>
{
    var _user = data.GetUser(id);
    if (_user == null)
    {
        Debug.WriteLine("Пользователь не найден");
        return Results.Content($"Пользователь не найден");
    }
    Debug.WriteLine(_user);
    return Results.Json(_user);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
