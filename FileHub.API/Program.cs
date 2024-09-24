using Core.Extensions; 
using FileHub.API.Extensions; 

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();
builder.AddCoreOptionConfiguration();
builder.AddLoggingContext();
builder.AddApplicationService();
builder.Services.AddControllers();
builder.AddCustomRateLimiter();
builder.AddRedisDatabase();
builder.AddAuth();


if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Docker")
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
 

if (app.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Docker")
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseRateLimiter(); 
app.UseHttpsRedirection();  
app.UseHsts();  

app.MapControllers(); 
app.Run();
 