using FileHub.API.Abstraction;
using FileHub.API.Configurations;
using FileHub.API.Infrastructure;
using FileHub.API.Validations;
using FileHub.API.Validations.Abstraction;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args); 

builder.Services.Configure<StorageConfigs>(builder.Configuration.GetSection(StorageConfigs.SectionName)); 
builder.Services.Configure<KmsConfigs>(builder.Configuration.GetSection(KmsConfigs.SectionName)); 
builder.Services.Configure<CacheConfigs>(builder.Configuration.GetSection(CacheConfigs.SectionName)); 
var storageConfigs = builder.Configuration.GetSection(StorageConfigs.SectionName).Get<StorageConfigs>();



builder.Services.AddSingleton<IFileValidation, FileValidator>();
builder.Services.AddSingleton<ICacheService, CacheService>();

if (storageConfigs?.UseMinio ?? false)
{ 
    builder.Services.AddSingleton<IStorageService, MinioStorageService>();
}
else
{
    builder.Services.AddSingleton<IStorageService, AwsStorageService>();
}

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10_485_760; // 10 MB 
});

builder.Services.AddControllers();

builder.Services.AddRateLimiter(options =>
{ 
    options.AddTokenBucketLimiter("FileUploadRateLimiterPolicy", configureOptions =>
    {
        configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        configureOptions.TokenLimit = 500;
        configureOptions.QueueLimit = 50;
        configureOptions.TokensPerPeriod = 500;
        configureOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
    });

    options.AddTokenBucketLimiter("FileReadRateLimiting", configureOptions =>
    {
        configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        configureOptions.TokenLimit = 1000;
        configureOptions.QueueLimit = 100;
        configureOptions.TokensPerPeriod = 1000;
        configureOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
    }); 
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var cacheOptions = sp.CreateScope().ServiceProvider.GetRequiredService<IOptionsMonitor<CacheConfigs>>(); 
    var configurations = new ConfigurationOptions
    {
        EndPoints = { cacheOptions.CurrentValue.Host },
        ConnectTimeout = (int)TimeSpan.FromSeconds(cacheOptions.CurrentValue.ConnectTimeout).TotalMilliseconds,
        SyncTimeout = (int)TimeSpan.FromSeconds(cacheOptions.CurrentValue.SyncTimeout).TotalMilliseconds,
        AbortOnConnectFail = false,
        ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan
            .FromSeconds(cacheOptions.CurrentValue.ReconnectRetryPolicy).TotalMilliseconds)
    }; 
    return ConnectionMultiplexer.Connect(configurations);
}); 

var app = builder.Build();

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseHsts();
 
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
} 

app.MapControllers();

app.Run();
 