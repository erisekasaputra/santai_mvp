using Amazon.KeyManagementService;
using Core.Configurations; 
using Core.Services.Interfaces;
using Core.Services;
using FluentValidation;
using FluentValidation.AspNetCore; 
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication;
using Core.Authentications;
using System.Linq;

namespace Core.Extensions;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddMediatorService<T>(this IServiceCollection services)
    {
        services.AddMediatR(e =>
        {
            e.RegisterServicesFromAssemblyContaining<T>();
        });

        return services;
    }


    public static IServiceCollection AddJsonEnumConverterBehavior(this IServiceCollection services)
    {
        services.Configure<JsonOptions>(options =>
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(
                namingPolicy: System.Text.Json.JsonNamingPolicy.CamelCase,
                allowIntegerValues: true));
        });

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return services;
    }

    public static AuthenticationBuilder AddGoogleSSO(this AuthenticationBuilder builder)
    {
        var googleConfiguration = builder.Services.BuildServiceProvider().GetService<IOptionsMonitor<GoogleSSOConfiguration>>()?.CurrentValue
          ?? throw new Exception("Please provide value for google configuratoin");

        builder.AddGoogle(googleOption =>
        {
            googleOption.ClientId = googleConfiguration?.ClientId ?? throw new Exception("Google client id can not be null");
            googleOption.ClientSecret = googleConfiguration?.ClientSecret ?? throw new Exception("Google client secret can not be null");
        });

        return builder;
    }


    public static AuthenticationBuilder AddAuth(this IServiceCollection services, params AuthenticationClient[] clients)
    {  
        string? authenticationSchema = clients?.Where(x => x.AuthenticationScheme == AuthenticationClientScheme.UserAuthenticationScheme).FirstOrDefault()?.AuthenticationScheme.ToString() ?? throw new Exception("Default authentication scheme is empty");

        var jwtOption = services.BuildServiceProvider().GetService<IOptionsMonitor<JwtConfiguration>>()
           ?? throw new Exception("Please provide value for message bus options");

        var jwt = jwtOption.CurrentValue;

        var authenticationBuilder = services.AddAuthentication(authOption =>
        { 
            authOption.DefaultAuthenticateScheme = !string.IsNullOrEmpty(authenticationSchema) ? authenticationSchema : throw new Exception("Default authentication scheme is empty"); 
            authOption.DefaultChallengeScheme = !string.IsNullOrEmpty(authenticationSchema) ? authenticationSchema : throw new Exception("Default authentication scheme is empty");
        });
         

        foreach(var client in clients)
        {
            authenticationBuilder.AddJwtBearer(client.AuthenticationScheme.ToString(), options =>
            {
                var secretKey = Encoding.UTF8.GetBytes(jwt?.SecretKey ?? throw new Exception("Secret key for jwt can not be empty"));

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true, 
                    ValidIssuer = jwt?.Issuer ?? throw new Exception("Issuer can not be null"),
                    ValidAudience = jwt?.Audience ?? throw new Exception("Audience can not be null"),
                    IssuerSigningKey = new SymmetricSecurityKey(secretKey)
                };
            });
        }
         
        services.AddAuthorization(options =>
        {
            foreach(var client in clients)
            {
                foreach(var policy in client.Policies)
                {
                    if (policy is not null)
                    {
                        options.AddPolicy(policy.PolicyName.ToString(), policyBuilder =>
                        {
                            policyBuilder.AuthenticationSchemes = [client.AuthenticationScheme.ToString()];
                            policyBuilder.RequireAuthenticatedUser();
                            policyBuilder.RequireRole(policy.PolicyRole.Select(x => x.ToString()));
                        });  
                    }
                }
            } 
        });

        return authenticationBuilder;
    }

    public static IServiceCollection AddRedisDatabase(this IServiceCollection services)
    {
        var cacheOptions = services.BuildServiceProvider().GetService<IOptionsMonitor<CacheConfiguration>>()
            ?? throw new Exception("Please provide value for database option");

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
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

        services.AddStackExchangeRedisCache(options =>
        {
            options.ConfigurationOptions = new ConfigurationOptions
            {

                EndPoints = { cacheOptions.CurrentValue.Host },
                ConnectTimeout = (int)TimeSpan.FromSeconds(cacheOptions.CurrentValue.ConnectTimeout).TotalMilliseconds,
                SyncTimeout = (int)TimeSpan.FromSeconds(cacheOptions.CurrentValue.SyncTimeout).TotalMilliseconds,
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan
                    .FromSeconds(cacheOptions.CurrentValue.ReconnectRetryPolicy).TotalMilliseconds)
            };
        });






        services.AddOutputCache(policy =>
        {
            policy.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(cacheOptions.CurrentValue.CacheLifeTime);
        })
        .AddStackExchangeRedisOutputCache(redisOptions =>
        {
            redisOptions.ConfigurationOptions = new ConfigurationOptions
            {

                EndPoints = { cacheOptions.CurrentValue.Host },
                ConnectTimeout = (int)TimeSpan.FromSeconds(cacheOptions.CurrentValue.ConnectTimeout).TotalMilliseconds,
                SyncTimeout = (int)TimeSpan.FromSeconds(cacheOptions.CurrentValue.SyncTimeout).TotalMilliseconds,
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan
                    .FromSeconds(cacheOptions.CurrentValue.ReconnectRetryPolicy).TotalMilliseconds)
            };
        });



        return services;
    }


    public static IServiceCollection AddSqlDatabaseContext<T>(this IServiceCollection services, bool isRetryable = false) where T : DbContext
    {
        var databaseOption = services.BuildServiceProvider().GetService<IOptionsMonitor<DatabaseConfiguration>>()
            ?? throw new Exception("Please provide value for database option");

        services.AddDbContext<T>(options =>
        {
            options.UseSqlServer(databaseOption.CurrentValue.ConnectionString, action =>
            {
                if (isRetryable)
                {
                    action.EnableRetryOnFailure();
                } 
                action.CommandTimeout(databaseOption.CurrentValue.CommandTimeout); 
            });
        });

        return services;
    }


    public static IServiceCollection AddValidation<T>(this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<T>();

        return services;
    } 

    public static WebApplicationBuilder AddLoggingContext(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        return builder;
    } 

    public static IServiceCollection AddDataEncryption(this IServiceCollection services, IConfiguration configuration)
    {
        bool production = false;
        if (production)
        {
            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonKeyManagementService>();
            services.AddSingleton<IEncryptionService>(sp =>
            {
                var kmsClient = sp.GetRequiredService<IAmazonKeyManagementService>();
                var kmsOptions = sp.GetRequiredService<IOptionsMonitor<EncryptionConfiguration>>();
                return new ThirdPartyEncryptionService(kmsClient, kmsOptions.CurrentValue.Id);
            });
        }
        else
        {
            services.AddSingleton<IEncryptionService>(sp =>
            {
                var kmsOptions = sp.GetRequiredService<IOptionsMonitor<EncryptionConfiguration>>();
                return new EncryptionService(kmsOptions.CurrentValue.SecretKey);
            });
        }
        services.AddSingleton<IHashService, HashService>();

        return services;
    }

    public static WebApplicationBuilder AddCoreOptionConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AccountServiceConfiguration>(builder.Configuration.GetSection(AccountServiceConfiguration.SectionName));
        builder.Services.Configure<CacheConfiguration>(builder.Configuration.GetSection(CacheConfiguration.SectionName));
        builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection(DatabaseConfiguration.SectionName));
        builder.Services.Configure<ElasticsearchConfiguration>(builder.Configuration.GetSection(ElasticsearchConfiguration.SectionName));
        builder.Services.Configure<EncryptionConfiguration>(builder.Configuration.GetSection(EncryptionConfiguration.SectionName));
        builder.Services.Configure<GoogleSSOConfiguration>(builder.Configuration.GetSection(GoogleSSOConfiguration.SectionName));
        builder.Services.Configure<IdempotencyConfiguration>(builder.Configuration.GetSection(IdempotencyConfiguration.SectionName));
        builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetSection(JwtConfiguration.SectionName));
        builder.Services.Configure<MessagingConfiguration>(builder.Configuration.GetSection(MessagingConfiguration.SectionName));
        builder.Services.Configure<OtpConfiguration>(builder.Configuration.GetSection(OtpConfiguration.SectionName));
        builder.Services.Configure<RateLimiterConfiguration>(builder.Configuration.GetSection(RateLimiterConfiguration.SectionName));
        builder.Services.Configure<RateLimiterRuleConfiguration>(builder.Configuration.GetSection(RateLimiterRuleConfiguration.SectionName));
        builder.Services.Configure<ReferralProgramConfiguration>(builder.Configuration.GetSection(ReferralProgramConfiguration.SectionName));
        builder.Services.Configure<StorageConfiguration>(builder.Configuration.GetSection(StorageConfiguration.SectionName)); 

        return builder;
    }
}
