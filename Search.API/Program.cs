using Core.Configurations;
using Core.Extensions; 
using Search.API.API; 
using Search.API.Domain.Repositories;
using Search.API.Infrastructure;
using Search.API.Infrastructure.Repositories; 

var builder = WebApplication.CreateBuilder(args);

builder.AddJsonEnumConverterBehavior();
builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<ElasticsearchConfiguration>(builder.Configuration.GetSection(ElasticsearchConfiguration.SectionName));


if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Staging")
{ 
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

builder.Services.AddScoped<ElasticsearchContext>();  
builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddHealthChecks(); 

var app = builder.Build();
 
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Staging")
{ 
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.MapHealthChecks("/health");
app.MapCatalogSearchApi();  
app.Run();