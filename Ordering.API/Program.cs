 
using Core.Extensions;
using Core.Middlewares;
using Ordering.API;
using Ordering.API.Extensions; 
using Ordering.Infrastructure; 

var builder = WebApplication.CreateBuilder(args);

builder.AddCoreOptionConfiguration();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
}); 
builder.Services.AddRouting(); 
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();


builder.AddJsonEnumConverterBehavior();
builder.AddLoggingContext();
builder.AddApplicationService();
builder.AddDataEncryption();
builder.AddMediatorService<IOrderAPIMarkerInterface>();
builder.AddRedisDatabase();
builder.AddSqlDatabaseContext<OrderDbContext>();
builder.AddMassTransitContext<OrderDbContext>();
builder.AddValidation<IOrderAPIMarkerInterface>();
builder.AddHttpClients();
builder.AddAuth();
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

app.MapControllers();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.MapHealthChecks("/health"); 

app.UseOutputCache();
app.Run();
