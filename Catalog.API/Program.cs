using Catalog.API;
using Catalog.API.API; 
using Catalog.API.Extensions;
using Catalog.API.Middewares;
using Catalog.Infrastructure;
using Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();

builder.Services.AddHttpContextAccessor();
builder.Services.AddJsonEnumConverterBehavior();
builder.Services.AddMediatorService<ICatalogAPIMarkerInterface>();
builder.Services.AddValidation<ICatalogAPIMarkerInterface>(); 
builder.Services.AddRouting();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSqlDatabaseContext<CatalogDbContext>(); 
builder.Services.AddMassTransitContext();
builder.Services.AddRedisDatabase();
builder.Services.AddApplicationService();
builder.Services.AddAuth();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<IdempotencyMiddleware>();
 
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