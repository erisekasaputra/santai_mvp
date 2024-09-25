using Core.Extensions;
using Core.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
builder.Services.AddRouting();
builder.Services.AddControllers();
builder.Services.AddDirectoryBrowser();
builder.Services.AddHealthChecks();
builder.Services.AddTransient<GlobalExceptionMiddleware>();

builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();
builder.AddJsonEnumConverterBehavior(); 
builder.AddRedisDatabase();

if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Staging")
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}
var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAllOrigins"); 
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Staging")
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
} 

app.UseStaticFiles();
app.MapControllers();
app.UseOutputCache();

app.Run(); 
