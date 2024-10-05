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
    public static WebApplicationBuilder AddMediatorService<T>(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(configure =>
        {
            configure.RegisterServicesFromAssemblyContaining<T>();
        });

        return builder;
    } 

    public static WebApplicationBuilder AddJsonEnumConverterBehavior(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<JsonOptions>(configure =>
        {
            configure.SerializerOptions.Converters.Add(new JsonStringEnumConverter(
                namingPolicy: System.Text.Json.JsonNamingPolicy.CamelCase,
                allowIntegerValues: true));
        });

        builder.Services.AddControllers()
            .AddJsonOptions(configure =>
            {
                configure.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        return builder;
    }

    public static AuthenticationBuilder AddGoogleSSO(this AuthenticationBuilder builder)
    {
        var options = builder.Services.BuildServiceProvider().GetService<IOptionsMonitor<GoogleSSOConfiguration>>()?.CurrentValue
          ?? throw new Exception("Google SSO options can no be null");

        builder.AddGoogle(configure =>
        {
            configure.ClientId = options?.ClientId ?? throw new Exception();
            configure.ClientSecret = options?.ClientSecret ?? throw new Exception();
        });

        return builder;
    }


    public static AuthenticationBuilder AddAuth(this WebApplicationBuilder builder)
    {    
        var options = builder.Configuration.GetSection(JwtConfiguration.SectionName).Get<JwtConfiguration>()
           ?? throw new Exception("JWT options can no be null"); 

        var authenticationBuilder = builder.Services.AddAuthentication(configure =>
        {
            configure.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            configure.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });


        authenticationBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, configure =>
        {
            var secretKey = Encoding.UTF8.GetBytes(options.SecretKey ?? throw new Exception("Secret key has not been set")); 
            configure.TokenValidationParameters = new TokenValidationParameters()
            {
                RequireExpirationTime = false,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = options.Issuer ?? throw new Exception("Issuer can not be null"),
                ValidAudience = options.Audience ?? throw new Exception("Audience can not be null"),
                IssuerSigningKey = new SymmetricSecurityKey(secretKey)
            };
        });

        builder.Services.AddAuthorization(configure =>
        {
            configure.AddPolicy(PolicyName.AdministratorUserOnlyPolicy.ToString(), policyBuilder =>
            { 
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.Administrator.ToString());
            });

            configure.AddPolicy(PolicyName.BusinessUserAndAdministratorUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.BusinessUser.ToString(), UserType.Administrator.ToString());
            });

            configure.AddPolicy(PolicyName.BusinessUserAndStaffUserAndAdministratorUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.BusinessUser.ToString(), UserType.StaffUser.ToString(), UserType.Administrator.ToString());
            });

            configure.AddPolicy(PolicyName.StaffUserAndAdministratorUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.StaffUser.ToString(), UserType.Administrator.ToString());
            });

            configure.AddPolicy(PolicyName.RegularUserAndAdministratorUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.RegularUser.ToString(), UserType.Administrator.ToString());
            });

            configure.AddPolicy(PolicyName.RegularUserOnlyPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.RegularUser.ToString());
            });

            configure.AddPolicy(PolicyName.MechanicUserOnlyPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.MechanicUser.ToString());
            });

            configure.AddPolicy(PolicyName.MechanicUserAndAdministratorUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.MechanicUser.ToString(), UserType.Administrator.ToString());
            });

            configure.AddPolicy(PolicyName.StaffUserOnlyPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.StaffUser.ToString());
            });

            configure.AddPolicy(PolicyName.ServiceToServiceAndAdministratorUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.ServiceToService.ToString(), UserType.Administrator.ToString());
            });

            configure.AddPolicy(PolicyName.ServiceToServiceOnlyPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.ServiceToService.ToString());
            });

            configure.AddPolicy(PolicyName.BusinessStaffRegularUserPolicy.ToString(), policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                policyBuilder.RequireRole(UserType.BusinessUser.ToString(), UserType.StaffUser.ToString(), UserType.RegularUser.ToString());
            });
        });

        return authenticationBuilder;
    }

    public static WebApplicationBuilder AddRedisDatabase(this WebApplicationBuilder builder)
    { 
        var options = builder.Configuration.GetSection(CacheConfiguration.SectionName).Get<CacheConfiguration>()
           ?? throw new Exception("Cache options can no be null"); 

        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var configurations = new ConfigurationOptions
            { 
                EndPoints = { options.Host },
                ConnectTimeout = (int)TimeSpan.FromSeconds(options.ConnectTimeout).TotalMilliseconds,
                SyncTimeout = (int)TimeSpan.FromSeconds(options.SyncTimeout).TotalMilliseconds,
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan
                    .FromSeconds(options.ReconnectRetryPolicy).TotalMilliseconds)
            };


            configurations.Ssl = options.Ssl;

            return ConnectionMultiplexer.Connect(configurations);
        });


        builder.Services.AddStackExchangeRedisCache(configure =>
        {
            var configurations = new ConfigurationOptions
            { 
                EndPoints = { options.Host },
                ConnectTimeout = (int)TimeSpan.FromSeconds(options.ConnectTimeout).TotalMilliseconds,
                SyncTimeout = (int)TimeSpan.FromSeconds(options.SyncTimeout).TotalMilliseconds,
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan
                    .FromSeconds(options.ReconnectRetryPolicy).TotalMilliseconds)
            };

            configurations.Ssl = options.Ssl;

            configure.ConfigurationOptions = configurations; 
        });


        builder.Services.AddOutputCache(configure =>
        {
            configure.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(options.CacheLifeTime);
        })
        .AddStackExchangeRedisOutputCache(configure =>
        {
            var configurations = new ConfigurationOptions
            {
                EndPoints = { options.Host },
                ConnectTimeout = (int)TimeSpan.FromSeconds(options.ConnectTimeout).TotalMilliseconds,
                SyncTimeout = (int)TimeSpan.FromSeconds(options.SyncTimeout).TotalMilliseconds,
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry((int)TimeSpan
                    .FromSeconds(options.ReconnectRetryPolicy).TotalMilliseconds)
            };
             
            configurations.Ssl = options.Ssl;

            configure.ConfigurationOptions = configurations;
        });  


        return builder;
    }


    public static WebApplicationBuilder AddSqlDatabaseContext<TDbContext>(this WebApplicationBuilder builder, bool isRetryable = false) where TDbContext : DbContext
    {
        var options = builder.Configuration.GetSection(DatabaseConfiguration.SectionName).Get<DatabaseConfiguration>()
          ?? throw new Exception("Database options can no be null");


        builder.Services.AddDbContext<TDbContext>(configure =>
        {
            configure.UseSqlServer(options.ConnectionString, action =>
            {
                if (isRetryable)
                {
                    action.EnableRetryOnFailure();
                }
                action.CommandTimeout(options.CommandTimeout);
            });
        });

        return builder;
    }


    public static WebApplicationBuilder AddValidation<T>(this WebApplicationBuilder builder)
    {
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddFluentValidationClientsideAdapters();
        builder.Services.AddValidatorsFromAssemblyContaining<T>();

        return builder;
    } 

    public static WebApplicationBuilder AddLoggingContext(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        return builder;
    }

    public static HostApplicationBuilder AddLoggingContext(this HostApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        return builder;
    }

    public static WebApplicationBuilder AddDataEncryption(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IEncryptionService>(configure =>
        {
            var kmsOptions = configure.GetRequiredService<IOptionsMonitor<EncryptionConfiguration>>();
            return new EncryptionService(kmsOptions.CurrentValue.SecretKey);
        });
        builder.Services.AddSingleton<IHashService, HashService>();

        return builder;
    }

    public static WebApplicationBuilder AddCoreOptionConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<OrderConfiguration>(builder.Configuration.GetSection(OrderConfiguration.SectionName));
        builder.Services.Configure<ProjectConfiguration>(builder.Configuration.GetSection(ProjectConfiguration.SectionName));
        builder.Services.Configure<SenangPayPaymentConfiguration>(builder.Configuration.GetSection(SenangPayPaymentConfiguration.SectionName));
        builder.Services.Configure<MinioConfiguration>(builder.Configuration.GetSection(MinioConfiguration.SectionName));
        builder.Services.Configure<AWSIAMConfiguration>(builder.Configuration.GetSection(AWSIAMConfiguration.SectionName));
        builder.Services.Configure<AWSSNSConfiguration>(builder.Configuration.GetSection(AWSSNSConfiguration.SectionName));
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
