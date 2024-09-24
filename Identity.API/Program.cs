using Core.Extensions;
using Core.Middlewares;
using Identity.API;  
using Identity.API.Extensions;
using Identity.API.Infrastructure; 
using Identity.API.SeedWork;  

var builder = WebApplication.CreateBuilder(args); 

builder.Configuration.AddEnvironmentVariables();
builder.AddCoreOptionConfiguration();
builder.AddLoggingContext(); 

builder.Services.AddControllers();  
builder.Services.AddRouting();
builder.Services.AddHttpContextAccessor();


builder.AddJsonEnumConverterBehavior(); 
builder.AddMediatorService<IIdentityMarkerInterface>();
builder.AddValidation<IIdentityMarkerInterface>(); 
builder.AddApplicationService();

 
builder.AddCustomRateLimiter(); 
builder.ConfigureOtpService(); 
builder.AddSqlDatabaseContext<ApplicationDbContext>(isRetryable: true); 
builder.AddMasstransitContext<ApplicationDbContext>();
builder.AddIdentityService();
builder.AddAuth().AddGoogleSSO();
builder.AddRedisDatabase();  


if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Docker")
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}


var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
//app.UseMiddleware<IdempotencyMiddleware>();

app.UseAuthentication(); 
app.UseAuthorization(); 

app.UseRateLimiter(); 

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(); 
    app.UseSwaggerUI();
    app.MapOpenApi();
}

//app.UseHsts(); 
//app.UseHttpsRedirection(); 

app.MapControllers(); 

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedDatabase.Initialize(services);
} 

app.Run(); 
