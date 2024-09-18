using Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();
builder.Services.AddJsonEnumConverterBehavior(); 
builder.Services.AddControllers();
builder.Services.AddDirectoryBrowser();
builder.Services.AddRedisDatabase();

var app = builder.Build();

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();
app.UseOutputCache();

app.Run(); 
