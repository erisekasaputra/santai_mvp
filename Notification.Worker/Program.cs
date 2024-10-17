using Core.Extensions; 
using Notification.Worker.Extensions;
using Notification.Worker.Infrastructure;
using Notification.Worker.Services; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
}); 

builder.Services.AddRouting();
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
app.UseRouting();
app.UseCors("AllowAllOrigins");
app.MapHub<ActivityHub>("/notification");
app.MapHealthChecks("/health");
app.Run();
