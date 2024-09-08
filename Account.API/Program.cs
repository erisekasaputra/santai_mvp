using Account.API.API;  
using Account.API.Extensions;
using Account.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.AddOptionConfiguration(); 
builder.AddLoggingContext();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi(); 
    builder.Services.AddSwaggerGen(); 
}

builder.Services.AddJsonEnumConverterBehavior(); 
builder.Services.AddAuth(); 
builder.Services.AddHttpContextAccessor(); 
builder.Services.AddMediatorService(); 
builder.Services.AddRedisDatabase();  
builder.Services.AddApplicationService(); 
builder.Services.AddValidation(); 
builder.Services.AddSqlDatabaseContext();   
builder.Services.AddMassTransitContext(); 
builder.Services.AddDataEncryption(builder.Configuration); 

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

app.Run();

public partial class Program;
 