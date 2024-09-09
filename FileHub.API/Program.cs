using Core.Extensions; 
using FileHub.API.Extensions;
using FileHub.API.SeedWorks;

var builder = WebApplication.CreateBuilder(args);

builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();
builder.Services.AddApplicationService();
builder.Services.AddControllers();
builder.Services.AddCustomRateLimiter();
builder.Services.AddRedisDatabase();
builder.Services.AddAuth([.. AuthPolicies.GetAuthClients()]);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter(); 
app.UseHttpsRedirection();  
app.UseHsts();  

app.MapControllers(); 
app.Run();
 