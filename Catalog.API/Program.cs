using Catalog.API;
using Catalog.API.API; 
using Catalog.API.Extensions; 
using Catalog.Infrastructure;
using Core.Extensions;
using Core.Middlewares;

var builder = WebApplication.CreateBuilder(args); 
builder.Configuration.AddEnvironmentVariables();
builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();

builder.Services.AddHttpContextAccessor();
builder.AddJsonEnumConverterBehavior();
builder.AddMediatorService<ICatalogAPIMarkerInterface>();
builder.AddValidation<ICatalogAPIMarkerInterface>(); 
builder.Services.AddRouting();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.AddSqlDatabaseContext<CatalogDbContext>(); 
builder.AddMassTransitContext<CatalogDbContext>();
builder.AddRedisDatabase();
builder.AddApplicationService();
builder.AddAuth();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<IdempotencyMiddleware>();

app.UseAuthentication();
app.UseAuthorization();


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
    app.MapOpenApi();
} 

const string groupName = "api/v1/catalog"; 

app.ItemRouter(groupName);     
app.CategoryRouter(groupName); 
app.BrandRouter(groupName); 

app.Run();

public partial class Program { }