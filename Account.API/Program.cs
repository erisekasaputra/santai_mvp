using Account.API;
using Account.API.API;
using Account.API.Applications.Services;
using Account.API.Extensions;  
using Account.Infrastructure;
using Core.Extensions;
using Core.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor(); 
builder.Configuration.AddEnvironmentVariables();

builder.AddCoreOptionConfiguration(); 
builder.AddLoggingContext();

if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Staging")
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

builder.Services.AddHealthChecks();
builder.AddJsonEnumConverterBehavior();
builder.AddAuth(); 
builder.AddMediatorService<IAccountAPIMarkerInterface>(); 
builder.AddRedisDatabase();
builder.AddApplicationService();
builder.AddValidation<IAccountAPIMarkerInterface>(); 
builder.AddSqlDatabaseContext<AccountDbContext>();   
builder.AddMassTransitContext<AccountDbContext>(); 
builder.AddDataEncryption(builder.Configuration);
builder.Services.AddSignalR();

var app = builder.Build(); 

app.UseMiddleware<GlobalExceptionMiddleware>(); 
app.UseMiddleware<IdempotencyMiddleware>(); 

app.UseAuthentication(); 
app.UseAuthorization();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Staging")
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection(); 
app.UseHsts(); 
app.UseOutputCache(); 

app.MapBusinessUserApi(); 
app.MapUserApi(); 
app.MapRegularUserApi(); 
app.MapMechanicUserApi(); 
app.MapFleetApi(); 

app.MapHub<LocationHub>("/location");
app.MapControllers();

app.MapHealthChecks("/health");

app.Run();

public partial class Program;
 