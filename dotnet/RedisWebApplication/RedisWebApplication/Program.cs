using Microsoft.EntityFrameworkCore;
using RedisWebApplication;
using StackExchange.Redis;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using Microsoft.Extensions.Caching.Distributed;


var builder = WebApplication.CreateBuilder(args);
string host = CredentialsSettings.hostname;   
string password = CredentialsSettings.password;

ConfigurationOptions conf = new ConfigurationOptions
{
    EndPoints = { host },
    User = "default",   
    Password = password,
};

ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(conf);

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

app.MapGet("/user/{id}", async (int id, UserService userService) =>
{
    User? user = await userService.GetUser(id);
    if (user != null) return $"User {user.Name}  Id={user.Id}  Age={user.Age}";
    return "User not found";
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
