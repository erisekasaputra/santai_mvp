 
using Core.Extensions;
using Ordering.API;
using Ordering.API.Extensions;
using Ordering.Infrastructure; 

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddJsonEnumConverterBehavior();
builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApplicationService();
builder.Services.AddDataEncryption(builder.Configuration);
builder.Services.AddMediatorService<IOrderAPIMarkerInterface>();
builder.Services.AddRedisDatabase();
builder.Services.AddControllers();
builder.Services.AddSqlDatabaseContext<OrderDbContext>();
builder.Services.AddMassTransitContext<OrderDbContext>();
builder.Services.AddValidation<IOrderAPIMarkerInterface>();
builder.Services.AddHttpClients();
builder.Services.AddOpenApi(); 
builder.Services.AddAuth();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization(); 
//app.UseMiddleware<IdempotencyMiddleware>();
app.MapControllers();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseHsts();
app.UseOutputCache();
app.Run();
