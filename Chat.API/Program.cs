using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Chat.API.Applications.Services;
using Chat.API.Applications.Services.Interfaces;
using Core.Configurations;
using Core.Extensions; 
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.AddJsonEnumConverterBehavior();
builder.AddCoreOptionConfiguration(); 
builder.AddDataEncryption(); 
builder.AddRedisDatabase(); 
builder.AddLoggingContext();
builder.Services.AddHealthChecks();

var options = builder.Configuration.GetSection(CacheConfiguration.SectionName).Get<CacheConfiguration>() ?? throw new Exception();

builder.Services.AddSignalR().AddStackExchangeRedis(configure =>
{ 
    var configurations = new ConfigurationOptions
    {
        EndPoints = { options.Host },
        ConnectTimeout = (int)TimeSpan.FromSeconds(options.ConnectTimeout).TotalMilliseconds,
        SyncTimeout = (int)TimeSpan.FromSeconds(options.SyncTimeout).TotalMilliseconds,
        AbortOnConnectFail = false,
        ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan
           .FromSeconds(options.ReconnectRetryPolicy).TotalMilliseconds),
        ChannelPrefix = RedisChannel.Literal("ChatAPI")
    };

    if (builder.Environment.IsProduction() || builder.Environment.IsStaging())
    {
        configurations.SslHost = options.Host.Split(':')[0];
        configurations.Ssl = true;
    }

    configure.Configuration = configurations;
});

builder.Services.AddAWSService<IAmazonDynamoDB>();
builder.Services.AddSingleton<IChatService, DynamoDBChatService>();

var app = builder.Build();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<ChatHub>("/chat");

app.MapHealthChecks("/health");

app.Run(); 