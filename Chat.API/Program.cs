using Chat.API;
using Chat.API.Applications.Services; 
using Chat.API.Extensions; 
using Core.Extensions;   

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddRouting();
builder.Services.AddHttpContextAccessor();


builder.AddLoggingContext();
builder.AddMediatorService<IChatAPIMarkerInterface>();
builder.AddJsonEnumConverterBehavior();
builder.AddCoreOptionConfiguration(); 
builder.AddDataEncryption(); 
builder.AddRedisDatabase();  
builder.AddApplicationService();
builder.AddAuth();

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHub<ChatHub>("/chat");
app.MapHealthChecks("/health");
app.Run(); 