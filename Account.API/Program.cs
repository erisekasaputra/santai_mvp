using Account.API.API;  
using Account.API.Extensions;
using Account.API.Middleware;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters
        .Add(new JsonStringEnumConverter(
            namingPolicy: System.Text.Json.JsonNamingPolicy.CamelCase, 
            allowIntegerValues: true));
});

builder.AddOptionConfiguration();

builder.AddLoggingContext();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();

    builder.Services.AddSwaggerGen(); 
}

builder.Services.AddHttpContextAccessor();

builder.Services.AddMediatorService();

builder.Services.AddRedisDatabase(); 

builder.Services.AddApplicationService();

builder.Services.AddValidation();

builder.Services.AddSqlDatabaseContext(); 

builder.Services.AddMassTransitContext();

builder.Services.AddDataEncryption(builder.Configuration); 

var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
    
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
  
//app.UseHsts();

app.UseMiddleware<IdempotencyMiddleware>(); 

app.MapBusinessUserApi();

app.MapUserApi();

app.MapRegularUserApi();

app.MapMechanicUserApi();

app.MapFleetApi(); 

app.Run();
 