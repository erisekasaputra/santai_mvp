using FluentValidation;
using FluentValidation.AspNetCore;
using Identity.API.Abstraction;
using Identity.API.Configs;
using Identity.API.Domain.Entities;
using Identity.API.Infrastructure;
using Identity.API.Middleware;
using Identity.API.SeedWork;
using Identity.API.Service;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using System.Threading.RateLimiting;  


var builder = WebApplication.CreateBuilder(args); 

builder.Services.AddTransient<GlobalExceptionMiddleware>();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddMediatR(configure =>
{  
    configure.RegisterServicesFromAssemblyContaining<Program>();
});

builder.Services.AddRateLimiter(options =>
{
    options.AddTokenBucketLimiter("AuthenticationRateRimiterPolicy", configureOptions =>
    {
        configureOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        configureOptions.TokenLimit = 500;
        configureOptions.QueueLimit = 50;
        configureOptions.TokensPerPeriod = 500;
        configureOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
    });
});

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(JwtConfig.SectionName));
builder.Services.Configure<GoogleConfig>(builder.Configuration.GetSection(GoogleConfig.SectionName));
builder.Services.Configure<DatabaseConfig>(builder.Configuration.GetSection(DatabaseConfig.SectionName));
builder.Services.Configure<CacheConfig>(builder.Configuration.GetSection(CacheConfig.SectionName));
builder.Services.Configure<OtpConfig>(builder.Configuration.GetSection(OtpConfig.SectionName));
builder.Services.Configure<FacebookConfig>(builder.Configuration.GetSection(FacebookConfig.SectionName));

var jwtOption = builder.Configuration.GetSection(JwtConfig.SectionName).Get<JwtConfig>();
var googleOptions = builder.Configuration.GetSection(GoogleConfig.SectionName).Get<GoogleConfig>(); 
var databaseOptions = builder.Configuration.GetSection(DatabaseConfig.SectionName).Get<DatabaseConfig>(); 
var eventBusOptions = builder.Configuration.GetSection(EventBusConfig.SectionName).Get<EventBusConfig>();
var otpOptions = builder.Configuration.GetSection(OtpConfig.SectionName).Get<OtpConfig>();
var facebookOptions = builder.Configuration.GetSection(FacebookConfig.SectionName).Get<FacebookConfig>();

builder.Services.AddSingleton<ITokenService, JwtTokenService>();

builder.Services.AddSingleton<ITokenService, JwtTokenService>();
 
builder.Services.AddSingleton<IJwtTokenValidator, JwtTokenValidator>();

if(builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IGoogleTokenValidator, MockGoogleTokenValidator>();
}

if(builder.Environment.IsProduction())
{
    builder.Services.AddSingleton<IGoogleTokenValidator, GoogleTokenValidator>();
}



builder.Services.AddSingleton<IOtpService, OtpService>();

builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddRouting();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer(); 

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{ 
    ArgumentNullException.ThrowIfNull(otpOptions);
  
    options.TokenLifespan = TimeSpan.FromSeconds(otpOptions.LockTimeSecond);
});


builder.Services.AddMassTransit(x =>
{
    if (eventBusOptions is null)
    {
        throw new Exception("Event bus options has not been set");
    } 

    x.AddEntityFrameworkOutbox<ApplicationDbContext>(o =>
    {
        o.DuplicateDetectionWindow = TimeSpan.FromSeconds(eventBusOptions.DuplicateDetectionWindows);
        o.QueryDelay = TimeSpan.FromSeconds(eventBusOptions.QueryDelay);
        o.QueryTimeout = TimeSpan.FromSeconds(eventBusOptions.QueryTimeout);
        o.QueryMessageLimit = eventBusOptions.QueryMessageLimit;
        o.UseSqlServer(); 
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, configure) =>
    {
        configure.Host(eventBusOptions.Host, host =>
        {
            host.Username(eventBusOptions.Username ?? "user");
            host.Password(eventBusOptions.Password ?? "user");
        });

        configure.UseMessageRetry(retryCfg =>
        {
            retryCfg.Interval(eventBusOptions.MessageRetryInterval, eventBusOptions.MessageRetryTimespan);
        });

        configure.UseTimeout(timeoutCfg =>
        {
            timeoutCfg.Timeout = TimeSpan.FromSeconds(eventBusOptions.MessageTimeout);
        });

        configure.ConfigureEndpoints(context); 
    }); 
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var databaseOption = databaseOptions ?? throw new Exception("Database connection string can not be empty"); 
    
    options.UseSqlServer(databaseOption.ConnectionString, optionBuilder =>
    {
        optionBuilder.CommandTimeout(databaseOption.CommandTimeout);
        optionBuilder.EnableRetryOnFailure(
            maxRetryCount: databaseOption.MaxRetryCount, 
            maxRetryDelay: TimeSpan.FromSeconds(databaseOption.MaxRetryDelay), 
            errorNumbersToAdd: null);
    });
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{  
    options.SignIn.RequireConfirmedPhoneNumber = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedAccount = false;

    options.User.RequireUniqueEmail = false;

    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>() 
.AddSignInManager<SignInManager<ApplicationUser>>()
.AddPasswordValidator<PasswordValidator<ApplicationUser>>()
.AddRoleManager<RoleManager<IdentityRole>>()
.AddRoles<IdentityRole>()
.AddDefaultTokenProviders()
.AddClaimsPrincipalFactory<ApplicationClaims>();

builder.Services.AddAuthentication(authOption =>
{
    authOption.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOption.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var secretKey = Encoding.UTF8.GetBytes(jwtOption?.SecretKey ?? throw new Exception("Secret key for jwt can not be empty"));

    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOption?.Issuer ?? throw new Exception("Issuer can not be null"),
        ValidAudience = jwtOption?.Audience ?? throw new Exception("Audience can not be null"),
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
})
.AddGoogle(googleOption =>
{
    googleOption.ClientId = googleOptions?.ClientId ?? throw new Exception("Google client id can not be null");
    googleOption.ClientSecret = googleOptions?.ClientSecret ?? throw new Exception("Google client secret can not be null");
});

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var cacheOptions = sp.CreateScope().ServiceProvider.GetRequiredService<IOptionsMonitor<CacheConfig>>();

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

 

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RegularUserPolicy", policy => 
    {
        policy.RequireRole("RegularUser");
    });
}); 

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedDatabase.Initialize(services);
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseRateLimiter();

app.UseHsts();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run(); 
