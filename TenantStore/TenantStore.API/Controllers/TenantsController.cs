namespace TenantStore.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenantStore.Application.DTOs.Tenant;
using TenantStore.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantsController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    /// <summary>
    /// Get all tenants (Super Admin only - for demo purposes, no auth)
    /// </summary>
    [HttpGet]
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
    /// Get tenant by subdomain
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
    /// Create new tenant (with admin user)
    /// </summary>
    [HttpPost]
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
    /// Update tenant theme color
    /// </summary>
    [HttpPut("{id}/theme")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTheme(Guid id, [FromBody] UpdateThemeDto dto)
    {
        var result = await _tenantService.UpdateThemeAsync(id, dto.ThemeColor);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return NoContent();
    }
}

public class UpdateThemeDto
{
    public string ThemeColor { get; set; } = string.Empty;
}
