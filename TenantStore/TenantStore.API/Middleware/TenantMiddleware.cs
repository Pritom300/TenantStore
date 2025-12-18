namespace TenantStore.API.Middleware;

using TenantStore.Application.Interfaces;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider, ITenantService tenantService)
    {
        var host = context.Request.Host.Host;

        // Extract subdomain from hostname
        // Examples: 
        // - alpha.localhost -> subdomain: "alpha"
        // - beta.localhost -> subdomain: "beta"
        // - alpha.yourdomain.com -> subdomain: "alpha"

        string? subdomain = null;

        // For localhost development (e.g., alpha.localhost)
        if (host.Contains("localhost"))
        {
            var parts = host.Split('.');
            if (parts.Length > 1)
            {
                subdomain = parts[0];
            }
        }
        else
        {
            // For production (e.g., alpha.yourdomain.com)
            var parts = host.Split('.');
            if (parts.Length >= 3) // Has subdomain
            {
                subdomain = parts[0];
            }
        }

        if (!string.IsNullOrEmpty(subdomain) && subdomain != "www")
        {
            var tenantResult = await tenantService.GetBySubdomainAsync(subdomain);

            if (tenantResult.IsSuccess && tenantResult.Data != null)
            {
                tenantProvider.SetTenant(tenantResult.Data.Id, tenantResult.Data.Subdomain);
            }
        }

        await _next(context);
    }
}

// Extension method for middleware registration
public static class TenantMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantMiddleware>();
    }
}