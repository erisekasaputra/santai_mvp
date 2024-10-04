using Chat.API.Applications.Services; 
using Chat.API.Extensions; 
using Core.Extensions;   

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.AddJsonEnumConverterBehavior();
builder.AddCoreOptionConfiguration(); 
builder.AddDataEncryption(); 
builder.AddRedisDatabase(); 
builder.AddLoggingContext();
builder.AddApplicationService();
builder.AddAuth();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/chat");
app.MapHealthChecks("/health");
app.Run(); 