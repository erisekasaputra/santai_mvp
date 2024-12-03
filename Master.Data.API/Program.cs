using Core.Extensions;
using Core.Middlewares;
using Master.Data.API.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.AddLoggingContext();
builder.Services.AddRouting();
builder.Services.AddControllers();
builder.Services.AddDirectoryBrowser();
builder.Services.AddHealthChecks();
builder.Services.AddTransient<GlobalExceptionMiddleware>();
builder.AddCoreOptionConfiguration(); 
builder.AddJsonEnumConverterBehavior(); 
builder.AddRedisDatabase();
builder.AddSqlDatabaseContext<MasterDbContext>();
builder.AddAuth();

if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}
var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAllOrigins"); 
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
} 

app.UseStaticFiles();
app.MapControllers();
app.UseOutputCache();
app.MapHealthChecks("/health");
app.Run(); 
