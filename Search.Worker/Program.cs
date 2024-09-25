using Core.Extensions;  
using Search.Worker.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddRouting();
builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();

builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();
builder.AddMasstransitContext();
builder.AddApplicationService();

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAllOrigins");
app.MapHealthChecks("/health");

app.Run();
