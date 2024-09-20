using Core.Extensions;
using Notification.Worker;
using Notification.Worker.Data;
using Notification.Worker.Extensions;

var builder = Host.CreateApplicationBuilder(args);
 
builder.AddLoggingContext();
builder.AddHostedCoreOptionConfiguration();
builder.Services.AddHttpContextAccessor();
builder.Configuration.AddEnvironmentVariables();   
builder.Services.AddApplicationService();
builder.Services.AddHostedService<Worker>();
builder.Services.AddRedisDatabase();
builder.Services.AddSqlDatabaseContext<NotificationDbContext>();
builder.Services.AddMassTransitContext<NotificationDbContext>(); 

var host = builder.Build();
host.Run();
