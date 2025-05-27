using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IPoductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IBaseRepository<Order, int>, OrderRepository>();
        services.AddScoped<IBaseRepository<Category, int>, CategoryRepository>();
        services.AddScoped<IBaseRepository<Product, int>, ProductRepository>();
    }
}
