using BugStore.Application.Interfaces;
using BugStore.Application.Mappings;
using BugStore.Application.Services;
using BugStore.Domain.Interfaces;
using BugStore.Handlers.Customers;
using BugStore.Infrastructure.Data;
using BugStore.Infrastructure.Repositories;
using BugStore.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
var envConnection = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
if (!string.IsNullOrEmpty(envConnection))
    connectionString = envConnection;

if (connectionString.Contains("Data Source="))
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
else
    builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<ProductProfle>();
    cfg.AddProfile<CustomerProfile>();
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetCustomerHandler).Assembly));

builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "BugStore API v1");
    options.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT");
if (port != null)
    app.Urls.Add($"http://0.0.0.0:{port}");
else
    app.Urls.Add("http://localhost:5000");

app.Run();
