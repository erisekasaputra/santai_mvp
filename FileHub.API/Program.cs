using Core.Extensions; 
using FileHub.API.Extensions; 

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();
builder.AddApplicationService();
builder.Services.AddControllers();
builder.AddCustomRateLimiter();
builder.AddRedisDatabase();
builder.AddAuth();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter(); 
app.UseHttpsRedirection();  
app.UseHsts();  

app.MapControllers(); 
app.Run();
 