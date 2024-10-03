using Account.API;
using Account.API.API;
using Account.API.Applications.Services;
using Account.API.Extensions;  
using Account.Infrastructure;
using Core.Extensions;
using Core.Middlewares;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddSignalR();
builder.Services.AddRouting();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();  
if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
} 

builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();
builder.AddJsonEnumConverterBehavior();
builder.AddAuth(); 
builder.AddMediatorService<IAccountAPIMarkerInterface>(); 
builder.AddRedisDatabase();
builder.AddApplicationService();
builder.AddValidation<IAccountAPIMarkerInterface>(); 
builder.AddSqlDatabaseContext<AccountDbContext>();   
builder.AddMassTransitContext<AccountDbContext>(); 
builder.AddDataEncryption();


var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAllOrigins");

app.UseMiddleware<GlobalExceptionMiddleware>(); 
app.UseMiddleware<IdempotencyMiddleware>(); 

app.UseAuthentication(); 
app.UseAuthorization();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}
  
app.UseOutputCache(); 

app.MapBusinessUserApi(); 
app.MapUserApi(); 
app.MapRegularUserApi(); 
app.MapMechanicUserApi(); 
app.MapFleetApi();
app.MapControllers();

app.MapHub<LocationHub>("/location");
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
 