using Core.Extensions;  
using Notification.Worker.Extensions;
using Notification.Worker.Infrastructure;
using Notification.Worker.Services; 

var builder = WebApplication.CreateBuilder(args); 

builder.Configuration.AddEnvironmentVariables();   
builder.Services.AddHttpContextAccessor();
builder.AddJsonEnumConverterBehavior();

builder.AddLoggingContext();
builder.AddCoreOptionConfiguration();
builder.AddApplicationService(); 
builder.AddRedisDatabase(); 
builder.AddMassTransitContext();
builder.AddAuth();
builder.AddSqlDatabaseContext<NotificationDbContext>();
builder.Services.AddHealthChecks();

var app = builder.Build();
 
app.MapHub<ActivityHub>("/notification");
app.MapHealthChecks("/health");
app.Run();
