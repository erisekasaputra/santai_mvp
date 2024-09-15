using Account.API;
using Account.API.API;
using Account.API.Applications.Services;
using Account.API.Extensions;
using Account.API.Middleware; 
using Account.Infrastructure;
using Core.Extensions; 

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.AddCoreOptionConfiguration(); 
builder.AddLoggingContext();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi(); 
    builder.Services.AddSwaggerGen(); 
}

builder.Services.AddJsonEnumConverterBehavior();
builder.Services.AddAuth(); 
builder.Services.AddHttpContextAccessor(); 
builder.Services.AddMediatorService<IAccountAPIMarkerInterface>(); 
builder.Services.AddRedisDatabase();
builder.Services.AddApplicationService();
builder.Services.AddValidation<IAccountAPIMarkerInterface>(); 
builder.Services.AddSqlDatabaseContext<AccountDbContext>();   
builder.Services.AddMassTransitContext<AccountDbContext>(); 
builder.Services.AddDataEncryption(builder.Configuration);
builder.Services.AddSignalR();

var app = builder.Build(); 

app.UseAuthentication(); 
app.UseAuthorization();
app.UseMiddleware<IdempotencyMiddleware>(); 
  
if (app.Environment.IsDevelopment())
{
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


app.MapHub<LocationHub>("/mechanic/location");
app.MapControllers();

app.Run();

public partial class Program;
 