using Catalog.API.API;
using Catalog.Infrastructure; 
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Catalog.API.Services; 
using Catalog.Domain.SeedWork; 
using MassTransit; 
using Catalog.Infrastructure.Helpers;
using Catalog.Contracts;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters
        .Add(new JsonStringEnumConverter(
            namingPolicy: System.Text.Json.JsonNamingPolicy.CamelCase,
            allowIntegerValues: true));
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddMediatR(builder =>
{ 
    builder.RegisterServicesFromAssemblies(typeof(Program).Assembly, typeof(ICatalogMakerInterface).Assembly);
});

builder.Logging.ClearProviders(); 
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information); 

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddRouting();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CatalogDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration["Database:SqlServer"], optionBuilder =>
    {
        optionBuilder.MigrationsAssembly("Catalog.API");
        optionBuilder.CommandTimeout(15);
    });
});

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<CatalogDbContext>(o =>
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

builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<MetaTableHelper>();
builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();  
 
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    app.UseSwagger();
    
    app.UseSwaggerUI(c => 
    {      

    });
    
    app.MapOpenApi();
} 

const string groupName = "api/v1/catalog"; 
app.ItemRouter(groupName);     
app.CategoryRouter(groupName); 
app.BrandRouter(groupName); 

app.Run();

public partial class Program { }