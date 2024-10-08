using IBB.Nesine.API.Common.Models;
using IBB.Nesine.Caching.Interfaces;
using IBB.Nesine.Caching.Providers;
using IBB.Nesine.Data;
using IBB.Nesine.Services.Consumers;
using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Jobs;
using IBB.Nesine.Services.Producers;
using IBB.Nesine.Services.Queue;
using IBB.Nesine.Services.Schedules;
using IBB.Nesine.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Web;
using Quartz;
using StackExchange.Redis;
using System.Text;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

var _rabbitMqQueueSettings = configuration.GetSection("RabbitMqQueueSettings");
builder.Services.Configure<RabbitMqQueueSettings>(options => _rabbitMqQueueSettings.Bind(options));

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();
    q.ScheduleJob<UpdateAvailableParksInfoJob>(trigger =>
        trigger
            .StartNow()
            .WithSimpleSchedule(x => x
                .WithIntervalInMinutes(10)
                .RepeatForever()));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
await SetIsAvailableJobSchedule.Start();

var cacheSettings = configuration.GetSection("CacheSettings").Get<CacheSettings>();



var logger = LogManager.Setup().LoadConfiguration(config => ConfigureNLog()).GetCurrentClassLogger();
// create a logger

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Debug); // Log Levels 0 - Trace, 1 - Debug, 2 - Information, 3 - Warning, 4 - Error, 5 - Critical, 6 - None,
builder.Host.UseNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddHttpClient<ApiServiceHelper>();
builder.Services.AddScoped<TokenHelper>();

var redisConnection = ConnectionMultiplexer.Connect(builder.Configuration["ConnectionString:RedisConnection"]);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);
builder.Services.AddSingleton<IParkService, ParkService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IDbProvider, DbProvider>();
builder.Services.AddSingleton<IJob, UpdateAvailableParksInfoJob>();
builder.Services.AddSingleton<RedisHelper>();
builder.Services.AddSingleton<MemCacheHelper>();
builder.Services.AddSingleton<RabbitMqProducer>();
builder.Services.AddSingleton<RabbitMqConsumer>();
builder.Services.AddSingleton<UpdateParksInfoConsumer>();
builder.Services.AddSingleton<UpdateAvailableParksInfoJobConsumer>();

if (cacheSettings.IsRedisEnabled)
{
    builder.Services.AddTransient<ICacheProvider, RedisCacheProvider>();
}

if (cacheSettings.IsMemCacheEnabled)
{
    builder.Services.AddMemoryCache(); // Adding MemoryCache
    builder.Services.AddSingleton<ICacheProvider, MemoryCacheProvider>(); // adding MemoryCacheProvider with DI
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]));

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = key
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<UpdateAvailableParksInfoJobConsumer>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfigureNLog()
{
    var config = new LoggingConfiguration();

    var databaseTarget = new DatabaseTarget("database")
    {
        ConnectionString = configuration["ConnectionString:DefaultConnection"],
        CommandText = @"INSERT INTO dbo.Logs (Level, Message, Exception) VALUES ( @Level, @Message, @Exception);"
    };
    databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Level", "${level}"));
    databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Message", "${message}"));
    databaseTarget.Parameters.Add(new DatabaseParameterInfo("@Exception", "${exception:format=tostring}"));
    var rule = new LoggingRule("*", NLog.LogLevel.Debug, databaseTarget);
    config.AddTarget(databaseTarget);
    config.AddRuleForOneLevel(NLog.LogLevel.Debug, databaseTarget);
    config.AddRuleForOneLevel(NLog.LogLevel.Warn, databaseTarget);
    //config.AddRuleForAllLevels(databaseTarget);
    LogManager.Configuration = config;
}

