using Core.Extensions;  
using Search.Worker.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();
builder.Configuration.AddEnvironmentVariables();
builder.AddMasstransitContext();
builder.AddApplicationService();   

var app = builder.Build();
app.Run();
