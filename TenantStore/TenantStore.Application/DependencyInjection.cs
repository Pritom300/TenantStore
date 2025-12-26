namespace TenantStore.Application;

using Microsoft.Extensions.DependencyInjection;
using TenantStore.Application.Interfaces;
using TenantStore.Application.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}