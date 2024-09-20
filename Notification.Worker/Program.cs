using Core.Extensions; 
using Notification.Worker.Data;
using Notification.Worker.Extensions;
using Notification.Worker.Services; 

var builder = WebApplication.CreateBuilder(args); 

builder.Services.AddJsonEnumConverterBehavior();
builder.AddLoggingContext();
builder.AddCoreOptionConfiguration();
builder.Services.AddHttpContextAccessor();
builder.Configuration.AddEnvironmentVariables();   
builder.Services.AddApplicationService(); 
builder.Services.AddRedisDatabase();
builder.Services.AddSqlDatabaseContext<NotificationDbContext>();
builder.Services.AddMassTransitContext<NotificationDbContext>();
builder.Services.AddAuth();

var app = builder.Build();

app.MapHub<ActivityHub>("/notification");

app.Run();
