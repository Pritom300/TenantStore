namespace TenantStore.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenantStore.Application.DTOs.Tenant;
using TenantStore.Application.Interfaces;
using TenantStore.Domain.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly IUnitOfWork _unitOfWork;

    public TenantsController(ITenantService tenantService, IUnitOfWork unitOfWork)
    {
        _tenantService = tenantService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Get all tenants (SuperAdmin only)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _tenantService.GetAllAsync();

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get tenant by ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _tenantService.GetByIdAsync(id);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get tenant by subdomain (Public for tenant detection)
    /// </summary>
    [HttpGet("by-subdomain/{subdomain}")]
    public async Task<IActionResult> GetBySubdomain(string subdomain)
    {
        var result = await _tenantService.GetBySubdomainAsync(subdomain);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Create new tenant (SuperAdmin only)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Create([FromBody] CreateTenantDto dto)
    {
        var result = await _tenantService.CreateAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Update tenant theme color (Admin or SuperAdmin)
    /// </summary>
    [HttpPut("{id}/theme")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpdateTheme(Guid id, [FromBody] UpdateThemeDto dto)
    {
        var result = await _tenantService.UpdateThemeAsync(id, dto.ThemeColor);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return NoContent();
    }

    /// <summary>
    /// Deactivate tenant (SuperAdmin only)
    /// </summary>
    [HttpPut("{id}/deactivate")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var tenant = await _tenantService.GetByIdAsync(id);
        if (!tenant.IsSuccess)
        {
            return NotFound(new { message = "Tenant not found" });
        }

        // Add deactivate logic in TenantService
        return NoContent();
    }

    /// <summary>
    /// Upgrade tenant subscription (Admin or SuperAdmin)
    /// </summary>
    [HttpPut("{id}/upgrade")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> UpgradeSubscription(Guid id, [FromBody] UpgradeSubscriptionDto dto)
    {
        var result = await _tenantService.UpgradeSubscriptionAsync(
            id,
            dto.Plan,
            dto.MaxProducts,
            dto.MaxUsers,
            dto.DurationMonths);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(new
        {
            message = $"Successfully upgraded to {dto.Plan} plan!",
            plan = dto.Plan,
            maxProducts = dto.MaxProducts,
            maxUsers = dto.MaxUsers
        });
    }

    [HttpGet("{id}/stats")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> GetTenantStats(Guid id)
    {
        var tenant = await _tenantService.GetByIdAsync(id);
        if (!tenant.IsSuccess)
        {
            return NotFound(new { message = "Tenant not found" });
        }

        var productCount = await _unitOfWork.Products
            .CountAsync(p => p.TenantId == id && !p.IsDeleted);

        var userCount = await _unitOfWork.Users
            .CountAsync(u => u.TenantId == id && !u.IsDeleted);

        return Ok(new
        {
            tenantId = id,
            tenantName = tenant.Data?.Name,
            subscriptionPlan = tenant.Data?.SubscriptionPlan,
            products = new
            {
                current = productCount,
                max = tenant.Data?.MaxProducts,
                remaining = tenant.Data?.MaxProducts - productCount,
                percentUsed = (productCount * 100.0 / tenant.Data?.MaxProducts)
            },
            users = new
            {
                current = userCount,
                max = tenant.Data?.MaxUsers,
                remaining = tenant.Data?.MaxUsers - userCount,
                percentUsed = (userCount * 100.0 / tenant.Data?.MaxUsers)
            }
        });
    }
}



public class UpdateThemeDto
{
    public string ThemeColor { get; set; } = string.Empty;
}


public class UpgradeSubscriptionDto
{
    public string Plan { get; set; } = string.Empty;
    public int MaxProducts { get; set; }
    public int MaxUsers { get; set; }
    public int DurationMonths { get; set; } = 12;
}