using Core.Extensions;
using Core.Middlewares;
using FileHub.API.Extensions; 

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddRouting();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();
builder.AddApplicationService();
builder.AddCustomRateLimiter();
builder.AddRedisDatabase();
builder.AddAuth();

if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Staging")
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAllOrigins");

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
 

if (app.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Staging")
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.MapHealthChecks("/health");
app.UseRateLimiter(); 

app.MapControllers(); 
app.Run();
 