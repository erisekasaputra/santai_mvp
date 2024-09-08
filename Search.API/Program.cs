using Core.Configurations;
using Microsoft.AspNetCore.Http.Json;
using Search.API.API; 
using Search.API.Domain.Repositories;
using Search.API.Infrastructure;
using Search.API.Infrastructure.Repositories;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters
        .Add(new JsonStringEnumConverter(
            namingPolicy: System.Text.Json.JsonNamingPolicy.CamelCase,
            allowIntegerValues: true));
});

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<ElasticsearchConfiguration>(builder.Configuration.GetSection("Elasticsearch"));

builder.Services.AddScoped<ElasticsearchContext>(); 

builder.Services.AddScoped<IItemRepository, ItemRepository>();
 

var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();

    app.MapOpenApi();
}

app.MapCatalogSearchApi();

app.UseHttpsRedirection(); 

app.Run();