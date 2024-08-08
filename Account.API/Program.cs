using Account.API.API;
using Account.API.Applications.Services;
using Account.API.Configurations;
using Account.API.Middleware;
using Account.Domain.SeedWork;
using Account.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Options;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); 
builder.Logging.AddDebug(); 

builder.Services.Configure<MasterReferralProgram>(builder.Configuration.GetSection(MasterReferralProgram.SectionName)); 
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(DatabaseSettings.DatabaseSection));

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(e =>
{
    e.RegisterServicesFromAssemblyContaining<Program>(); 
}); 

builder.Services.AddScoped<AppService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddFluentValidationAutoValidation(); 
builder.Services.AddFluentValidationClientsideAdapters();  
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddScoped<IIdempotencyService, IdempotencyService>();   
builder.Services.AddDbContext<AccountDbContext>((serviceProvider, options) =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value; 
    options.UseSqlServer(settings.ConnectionString, action =>
    {
        action.CommandTimeout(settings.CommandTimeOut);
           
        action.MigrationsAssembly(typeof(Program).Assembly.GetName().Name); 
    }).LogTo(Console.WriteLine, LogLevel.Information); 
});

if(false)
{ 
    builder.Services.AddMassTransit(x =>
    {
        x.AddEntityFrameworkOutbox<AccountDbContext>(o =>
        {
            o.DuplicateDetectionWindow = TimeSpan.FromSeconds(30);
            o.QueryDelay = TimeSpan.FromSeconds(1);
            o.QueryTimeout = TimeSpan.FromSeconds(30);
            o.QueryMessageLimit = 100;
            o.UseSqlServer();
            o.UseBusOutbox();
        });

        x.UsingRabbitMq((context, configure) =>
        {
            configure.Host(builder.Configuration["RabbitMQ:Host"]!, host =>
            {
                host.Username(builder.Configuration["RabbitMQ:Username"] ?? "user");
                host.Password(builder.Configuration["RabbitMQ:Password"] ?? "user");
            });

            configure.UseMessageRetry(retryCfg =>
            {
                retryCfg.Interval(3, TimeSpan.FromSeconds(2));
            });

            configure.UseTimeout(timeoutCfg =>
            {
                timeoutCfg.Timeout = TimeSpan.FromSeconds(5);
            });

            configure.ConfigureEndpoints(context);
        });
    });
}

var app = builder.Build();

app.UseMiddleware<IdempotencyMiddleware>(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
} 

app.MapBusinessUserApi();
app.MapUserApi();

app.Run();
 