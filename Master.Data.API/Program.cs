using Core.Extensions;
using Core.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();
builder.AddJsonEnumConverterBehavior(); 
builder.Services.AddControllers();
builder.Services.AddDirectoryBrowser();
builder.AddRedisDatabase();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHsts();
app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapControllers();
app.UseOutputCache();

app.Run(); 
