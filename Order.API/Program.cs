using Order.API.Extensions;
using Order.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.AddOptionConfiguration(); 
builder.AddLoggingContext(); 

builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationService();
builder.Services.AddMediatorService();
builder.Services.AddRedisDatabase();
builder.Services.AddJsonEnumConverterBehavior(); 
builder.Services.AddControllers(); 
builder.Services.AddSqlDatabaseContext();
builder.Services.AddMassTransitContext();
builder.Services.AddHttpClients();   
builder.Services.AddOpenApi();
builder.Services.AddDataEncryption(builder.Configuration);
builder.Services.AddAuth();

var app = builder.Build();
//app.UseAuthentication();
//app.UseAuthorization();

app.UseMiddleware<IdempotencyMiddleware>(); 
app.MapControllers();  
if (app.Environment.IsDevelopment())
{ 
    app.MapOpenApi();
} 
app.UseHttpsRedirection(); 
app.UseHsts();  
app.UseOutputCache();   
app.Run();
 