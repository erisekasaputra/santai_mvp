using Account.API.API;  
using Account.API.Extensions;
using Account.API.Middleware;   

var builder = WebApplication.CreateBuilder(args);

builder.AddOptionConfiguration();

builder.AddLoggingContext();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();

    builder.Services.AddSwaggerGen(); 
}

builder.Services.AddHttpContextAccessor();

builder.Services.AddMediatorService();

builder.Services.AddApplicationService();

builder.Services.AddRedisDatabase(); 

builder.Services.AddValidation();

builder.Services.AddSqlDatabaseContext(); 

builder.Services.AddMassTransitContext();

var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
    
    app.MapOpenApi();
}

app.UseHttpsRedirection();
  
app.UseHsts();

app.UseMiddleware<IdempotencyMiddleware>(); 

app.MapBusinessUserApi();

app.MapUserApi();

app.MapRegularUserApi();

app.UseHealthChecks("/health");

app.Run();
 