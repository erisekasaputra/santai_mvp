using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Order.API;
using Order.API.Applications.Services;
using Order.API.Applications.Services.Interfaces;
using Order.API.Configurations;
using Order.API.CustomDelegate;
using Order.Domain.Interfaces;
using Order.Domain.SeedWork;
using Order.Infrastructure;
using Order.Infrastructure.Services;
using System.Text.Json.Serialization;

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


builder.Services.AddControllers();

builder.Services.Configure<AccountServiceClientConfiguration>(builder.Configuration.GetSection(AccountServiceClientConfiguration.SectionName));

builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection(DatabaseConfiguration.SectionName));

var accountServiceClientConfig = builder.Configuration.GetSection(AccountServiceClientConfiguration.SectionName).Get<AccountServiceClientConfiguration>();

var databaseConfig = builder.Configuration.GetSection(DatabaseConfiguration.SectionName).Get<DatabaseConfiguration>();

builder.Services.AddMediatR(x =>
{
    x.RegisterServicesFromAssemblyContaining<IOrderApiMarkerInterface>();
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<TokenJwtHandler>();

builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddHttpClient<IAccountService, AccountService>(client =>
{
    client.BaseAddress = new Uri(accountServiceClientConfig?.Host ?? throw new Exception("Account service client host is not set"));
})
.AddHttpMessageHandler<TokenJwtHandler>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddDbContext<OrderDbContext>(options =>
{ 
    options.UseSqlServer(databaseConfig?.ConnectionString ?? throw new Exception("Database connection string is not set"), sqlServerOptions =>
    {
        sqlServerOptions.MigrationsAssembly(typeof(OrderDbContext).Assembly);
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
 
app.Run();
 