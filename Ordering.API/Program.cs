 
using Core.Extensions;
using Core.Middlewares;
using Ordering.API;
using Ordering.API.Extensions; 
using Ordering.Infrastructure; 

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
builder.AddCoreOptionConfiguration();


builder.AddJsonEnumConverterBehavior();
builder.AddLoggingContext();
builder.AddApplicationService();
builder.AddDataEncryption(builder.Configuration);
builder.AddMediatorService<IOrderAPIMarkerInterface>();
builder.AddRedisDatabase();
builder.AddSqlDatabaseContext<OrderDbContext>();
builder.AddMassTransitContext<OrderDbContext>();
builder.AddValidation<IOrderAPIMarkerInterface>();
builder.AddHttpClients();
builder.AddAuth();
builder.Services.AddHealthChecks();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseRouting();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<IdempotencyMiddleware>();
  
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.MapHealthChecks("/health"); 

app.UseOutputCache();
app.Run();
