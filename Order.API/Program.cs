using Order.API.Configurations;
using Order.API.CustomDelegate;
using Order.Domain.Interfaces;
using Order.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<AccountServiceClientConfiguration>(builder.Configuration.GetSection(AccountServiceClientConfiguration.SectionName));

var accountServiceClientConfig = builder.Configuration.GetSection(AccountServiceClientConfiguration.SectionName).Get<AccountServiceClientConfiguration>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<TokenJwtHandler>();

builder.Services.AddHttpClient<IAccountService, AccountService>(client =>
{
    client.BaseAddress = new Uri(accountServiceClientConfig?.Host ?? throw new Exception("Account service client host is not set"));
})
.AddHttpMessageHandler<TokenJwtHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
 
app.Run();
 