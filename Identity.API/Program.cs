using Core.Extensions; 
using Identity.API;  
using Identity.API.Extensions;
using Identity.API.Infrastructure; 
using Identity.API.SeedWork;  

var builder = WebApplication.CreateBuilder(args); 

builder.Configuration.AddEnvironmentVariables();
builder.AddCoreOptionConfiguration();
builder.AddLoggingContext(); 


builder.Services.AddHttpContextAccessor();
builder.Services.AddJsonEnumConverterBehavior(); 
builder.Services.AddHttpContextAccessor();  

builder.Services.AddMediatorService<IIdentityMarkerInterface>();
builder.Services.AddValidation<IIdentityMarkerInterface>(); 
builder.Services.AddApplicationService();

builder.Services.AddControllers();  
builder.Services.AddRouting();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();
builder.Services.AddCustomRateLimiter(); 
builder.Services.ConfigureOtpService(); 
builder.Services.AddSqlDatabaseContext<ApplicationDbContext>(isRetryable: true); 
builder.Services.AddMasstransitContext<ApplicationDbContext>();
builder.Services.AddIdentityService();
builder.Services.AddAuth().AddGoogleSSO();  


builder.Services.AddRedisDatabase();  

var app = builder.Build();

//app.UseMiddleware<GlobalExceptionMiddleware>();
//app.UseMiddleware<IdempotencyMiddleware>();  
app.UseRateLimiter(); 
app.UseHsts(); 
app.UseSwagger(); 
app.UseSwaggerUI(); 
app.UseHttpsRedirection(); 
app.UseAuthentication(); 
app.UseAuthorization(); 
app.MapControllers(); 

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedDatabase.Initialize(services);
} 

app.Run(); 
