namespace TenantStore.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TenantStore.Application.DTOs.Auth;
using TenantStore.Application.Interfaces;
using TenantStore.Infrastructure.Data;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITenantProvider _tenantProvider;
    private readonly ApplicationDbContext _db;
    public AuthController(IAuthService authService, ITenantProvider tenantProvider,ApplicationDbContext db)
    {
        _authService = authService;
        _tenantProvider = tenantProvider;
        _db = db;
    }

    /// <summary>
    /// User login for the current tenant
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        //test of mine
        var host = Request.Host.Host;
        var subdomain = host.Contains('.') ? host.Split('.')[0] : host;
        var tenant = await _db.Tenants.FirstOrDefaultAsync(t => t.Subdomain == subdomain);
        //test of end mine
        if (_tenantProvider.TenantId == null)
        {
            return BadRequest(new { message = "Tenant not found. Please use the correct subdomain." });
        }

        var result = await _authService.LoginAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// User registration for the current tenant
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (_tenantProvider.TenantId == null)
        {
            return BadRequest(new { message = "Tenant not found. Please use the correct subdomain." });
        }

        var result = await _authService.RegisterAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get current tenant info (no authentication required)
    /// </summary>
    [HttpGet("tenant-info")]
    public IActionResult GetTenantInfo()
    {
        if (_tenantProvider.TenantId == null)
        {
            return NotFound(new { message = "Tenant not found" });
        }

        return Ok(new
        {
            tenantId = _tenantProvider.TenantId,
            subdomain = _tenantProvider.TenantSubdomain
        });
    }
}