using Core.Configurations;
using Core.Extensions; 
using Search.API.API; 
using Search.API.Domain.Repositories;
using Search.API.Infrastructure;
using Search.API.Infrastructure.Repositories; 

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<ElasticsearchConfiguration>(builder.Configuration.GetSection("Elasticsearch")); 

builder.Services.AddOpenApi(); 
builder.Services.AddSwaggerGen();
builder.Services.AddJsonEnumConverterBehavior();

builder.Services.AddScoped<ElasticsearchContext>();  
builder.Services.AddScoped<IItemRepository, ItemRepository>();
 
var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();

    app.MapOpenApi();
}

app.UseHsts();

app.MapCatalogSearchApi();

app.UseHttpsRedirection(); 

app.Run();