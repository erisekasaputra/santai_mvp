using Catalog.API.API; 
using Catalog.API.Extensions;
using Catalog.API.Middewares;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.AddOptionConfiguration();
builder.AddLoggingContext();

builder.Services.AddHttpContextAccessor();
builder.Services.AddJsonEnumConverterBehavior();
builder.Services.AddMediatorService();
builder.Services.AddValidation(); 
builder.Services.AddRouting();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSqlDatabaseContext(); 
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