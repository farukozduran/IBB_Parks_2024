using IBB.Nesine.Data;
using IBB.Nesine.Services.Helpers;
using IBB.Nesine.Services.Interfaces;
using IBB.Nesine.Services.Jobs;
using IBB.Nesine.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using System.Text;
using NLog;
using NLog.Targets;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using NLog.Config;
using Microsoft.Extensions.Options;
using IBB.Nesine.Caching;
var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

//builder.Services.AddQuartz(q =>
//{
//    q.UseMicrosoftDependencyInjectionJobFactory();
//    q.ScheduleJob<UpdateAvailableParksInfoJob>(trigger =>
//        trigger
//            //.StartNow()
//            .WithSimpleSchedule(x => x
//                .WithIntervalInMinutes(10)
//                .RepeatForever()));
//});

//builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

//await SetIsAvailableJobSchedule.Start();

var logger = LogManager.Setup().LoadConfiguration(config => ConfigureNLog()).GetCurrentClassLogger();
// create a logger

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Debug);
builder.Host.UseNLog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddHttpClient<ApiServiceHelper>();

builder.Services.AddSingleton<IParkService, ParkService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IDbProvider, DbProvider>();
builder.Services.AddSingleton<IJob, UpdateAvailableParksInfoJob>();
builder.Services.AddSingleton<ICacheProvider, RedisCacheProvider>();

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

//builder.Services.AddAuthorization();

var app = builder.Build();

//app.Use(async (context, next) =>
//{
//    if (!context.User.Identity.IsAuthenticated && context.Request.Path.StartsWithSegments("/api"))
//    {
//        context.Response.Redirect("/api/Auth/Login");
//        return;
//    }

//    await next.Invoke();
//});



// Configure the HTTP request pipeline.
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
    config.AddRuleForAllLevels(databaseTarget);
    LogManager.Configuration = config;
}

