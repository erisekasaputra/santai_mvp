using Microsoft.EntityFrameworkCore;
using Order.API;
using Order.API.Configurations;
using Order.API.CustomDelegate;
using Order.Domain.Interfaces;
using Order.Infrastructure;
using Order.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddHttpClient<IAccountService, AccountService>(client =>
{
    client.BaseAddress = new Uri(accountServiceClientConfig?.Host ?? throw new Exception("Account service client host is not set"));
})
.AddHttpMessageHandler<TokenJwtHandler>();

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(databaseConfig?.ConnectionString ?? throw new Exception("Database connection string is not set"));
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
 