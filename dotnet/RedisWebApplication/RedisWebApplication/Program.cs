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
string host = CredentialsSettings.hostname;   
string password = CredentialsSettings.password;

ConfigurationOptions conf = new ConfigurationOptions
{
    EndPoints = { { host, 15653 } },
    User = "default",     //"#2678700",
    Password = password,
    //Ssl = true,
    //SslProtocols= System.Security.Authentication.SslProtocols.Tls12
};

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(conf);
//IDatabase cacheDb = redis.GetDatabase(); 


string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connection));
builder.Services.AddTransient<UserService>();
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = host;
    options.InstanceName = "default"; 
});
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(conf));


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
    var x = Results.Json(_user);
    Debug.WriteLine("");
    Debug.WriteLine("");
    Debug.WriteLine("----------------------------------");
    Debug.WriteLine(x);
    Debug.WriteLine(_user);
    Debug.WriteLine("------------------------------------");
    Debug.WriteLine("");
    Debug.WriteLine("");
    //return Results.Json(_user);
    return Results.Json(_user);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
