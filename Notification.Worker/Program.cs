using Core.Extensions; 
using Notification.Worker.Data;
using Notification.Worker.Extensions;
using Notification.Worker.Services; 

var builder = WebApplication.CreateBuilder(args); 

builder.Configuration.AddEnvironmentVariables();   
builder.Services.AddHttpContextAccessor();
builder.AddJsonEnumConverterBehavior();

builder.AddLoggingContext();
builder.AddCoreOptionConfiguration();
builder.AddApplicationService(); 
builder.AddRedisDatabase();
builder.AddSqlDatabaseContext<NotificationDbContext>();
builder.AddMassTransitContext<NotificationDbContext>();
builder.AddAuth();

var app = builder.Build();
 
app.MapHub<ActivityHub>("/notification"); 
app.Run();
