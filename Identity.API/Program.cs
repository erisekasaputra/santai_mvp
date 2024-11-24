using Core.Extensions;
using Core.Middlewares;
using Identity.API;
using Identity.API.Extensions;
using Identity.API.Infrastructure;
using Identity.API.SeedWork;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Configuration.AddEnvironmentVariables(); 
builder.Services.AddRouting();
builder.Services.AddControllers();  
builder.Services.AddHttpContextAccessor();
 

builder.AddCoreOptionConfiguration();
builder.AddLoggingContext(); 
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
builder.Services.AddHealthChecks();

if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
} 

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAllOrigins"); 

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<IdempotencyMiddleware>();

app.UseAuthentication(); 
app.UseAuthorization(); 

app.UseRateLimiter();

app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger(); 
    app.UseSwaggerUI();
    app.MapOpenApi();
} 

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedDatabase.Initialize(services);
} 

app.Run(); 
