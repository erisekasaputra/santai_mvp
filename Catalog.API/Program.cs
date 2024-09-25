using Catalog.API;
using Catalog.API.API; 
using Catalog.API.Extensions; 
using Catalog.Infrastructure;
using Core.Extensions;
using Core.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Staging")
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}
builder.Services.AddRouting();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();
 

builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();
builder.AddJsonEnumConverterBehavior();
builder.AddMediatorService<ICatalogAPIMarkerInterface>();
builder.AddValidation<ICatalogAPIMarkerInterface>();


builder.AddSqlDatabaseContext<CatalogDbContext>(); 
builder.AddMassTransitContext<CatalogDbContext>();
builder.AddRedisDatabase();
builder.AddApplicationService();
builder.AddAuth(); 

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAllOrigins");

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<IdempotencyMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Staging")
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