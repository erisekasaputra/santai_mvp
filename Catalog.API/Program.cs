using Catalog.API.API; 
using Catalog.Infrastructure; 
using Catalog.Infrastructure.Repositories; 
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Catalog.API.Validators;
using Catalog.API.Services;
using Catalog.Domain.Aggregates.ItemAggregate;
using Catalog.Domain.Aggregates.CategoryAggregate;
using Catalog.Domain.SeedWork;

var builder = WebApplication.CreateBuilder(args); 

builder.Services.AddHttpContextAccessor();

builder.Services.AddMediatR(builder =>
{
    builder.RegisterServicesFromAssemblyContaining<Program>();
});

builder.Logging.ClearProviders();

builder.Logging.AddConsole();
 
builder.Logging.AddDebug();

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddValidatorsFromAssemblyContaining<CreateItemCommandValidator>();

builder.Services.AddDbContext<CatalogDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), optionBuilder =>
    {
        optionBuilder.MigrationsAssembly("Catalog.API");
        optionBuilder.CommandTimeout(30);
    }); 
}); 

builder.Services.AddRouting();

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ApplicationService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IItemRepository, ItemRepository>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); 
 
var app = builder.Build();  

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    app.UseSwagger();
    
    app.UseSwaggerUI(c => 
    {      
    });
    
    app.MapOpenApi();
} 

app.CatalogRouter("api/v1/catalog");     

app.Run(); 