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
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Core.SeedWorks;
using Core.Enumerations;
using Microsoft.Extensions.Hosting;

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


    public static AuthenticationBuilder AddAuth(this IServiceCollection services)
    {    
        var jwtOption = services.BuildServiceProvider().GetService<IOptionsMonitor<JwtConfiguration>>()
           ?? throw new Exception("Please provide value for message bus options");

        var jwt = jwtOption.CurrentValue;

        var authenticationBuilder = services.AddAuthentication(authOption =>
        { 
            authOption.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authOption.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });


        authenticationBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            var secretKey = Encoding.UTF8.GetBytes(jwt?.SecretKey ?? throw new Exception("Secret key for jwt can not be empty"));

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                RequireExpirationTime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt?.Issuer ?? throw new Exception("Issuer can not be null"),
                ValidAudience = jwt?.Audience ?? throw new Exception("Audience can not be null"),
                IssuerSigningKey = new SymmetricSecurityKey(secretKey)
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(PolicyName.AdministratorUserOnlyPolicy.ToString(), policyBuilder =>
            { 
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.Administrator.ToString());
            });

            options.AddPolicy(PolicyName.BusinessUserAndAdministratorUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.BusinessUser.ToString(), UserType.Administrator.ToString());
            });

            options.AddPolicy(PolicyName.BusinessUserAndStaffUserAndAdministratorUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.BusinessUser.ToString(), UserType.StaffUser.ToString(), UserType.Administrator.ToString());
            });

            options.AddPolicy(PolicyName.StaffUserAndAdministratorUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.StaffUser.ToString(), UserType.Administrator.ToString());
            });

            options.AddPolicy(PolicyName.RegularUserAndAdministratorUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.RegularUser.ToString(), UserType.Administrator.ToString());
            });

            options.AddPolicy(PolicyName.RegularUserOnlyPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.RegularUser.ToString());
            });

            options.AddPolicy(PolicyName.MechanicUserOnlyPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.MechanicUser.ToString());
            });

            options.AddPolicy(PolicyName.MechanicUserAndAdministratorUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.MechanicUser.ToString(), UserType.Administrator.ToString());
            });

            options.AddPolicy(PolicyName.StaffUserOnlyPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.StaffUser.ToString());
            });

            options.AddPolicy(PolicyName.ServiceToServiceAndAdministratorUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.ServiceToService.ToString(), UserType.Administrator.ToString());
            });

            options.AddPolicy(PolicyName.ServiceToServiceOnlyPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.ServiceToService.ToString());
            });
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


    public static IServiceCollection AddSqlDatabaseContext<TDbContext>(this IServiceCollection services, bool isRetryable = false) where TDbContext : DbContext
    {
        var databaseOption = services.BuildServiceProvider().GetService<IOptionsMonitor<DatabaseConfiguration>>()
            ?? throw new Exception("Please provide value for database option");

        services.AddDbContext<TDbContext>(options =>
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
        builder.Services.Configure<MasterDataServiceConfiguration>(builder.Configuration.GetSection(MasterDataServiceConfiguration.SectionName));
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
        builder.Services.Configure<CatalogServiceConfiguration>(builder.Configuration.GetSection(CatalogServiceConfiguration.SectionName));
        builder.Services.Configure<SafelyShutdownConfiguration>(builder.Configuration.GetSection(SafelyShutdownConfiguration.SectionName));
        return builder;
    }

    public static HostApplicationBuilder AddHostedCoreOptionConfiguration(this HostApplicationBuilder builder)
    {
        builder.Services.Configure<AccountServiceConfiguration>(builder.Configuration.GetSection(AccountServiceConfiguration.SectionName));
        builder.Services.Configure<MasterDataServiceConfiguration>(builder.Configuration.GetSection(MasterDataServiceConfiguration.SectionName));
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
        builder.Services.Configure<CatalogServiceConfiguration>(builder.Configuration.GetSection(CatalogServiceConfiguration.SectionName));
        builder.Services.Configure<SafelyShutdownConfiguration>(builder.Configuration.GetSection(SafelyShutdownConfiguration.SectionName));
        return builder;
    }
}
