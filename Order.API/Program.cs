using Order.API.Extensions;
using Order.API.Middlewares;
using Core.Extensions;
using Order.API;
using Order.Infrastructure;
using Order.API.SeedWorks;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.AddCoreOptionConfiguration(); 
builder.AddLoggingContext(); 

builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationService();
builder.Services.AddMediatorService<IOrderApiMarkerInterface>();
builder.Services.AddRedisDatabase();
builder.Services.AddJsonEnumConverterBehavior(); 
builder.Services.AddControllers(); 
builder.Services.AddSqlDatabaseContext<OrderDbContext>();
builder.Services.AddMassTransitContext<OrderDbContext>();
builder.Services.AddValidation<IOrderApiMarkerInterface>();
builder.Services.AddHttpClients();   
builder.Services.AddOpenApi();
builder.Services.AddDataEncryption(builder.Configuration);
builder.Services.AddAuth();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

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
 