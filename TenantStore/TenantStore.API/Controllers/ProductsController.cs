namespace TenantStore.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenantStore.Application.DTOs.Product;
using TenantStore.Application.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ITenantProvider _tenantProvider;

    public ProductsController(IProductService productService, ITenantProvider tenantProvider)
    {
        _productService = productService;
        _tenantProvider = tenantProvider;
    }

    /// <summary>
    /// Get all products for current tenant
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (_tenantProvider.TenantId == null)
        {
            return BadRequest(new { message = "Tenant not found" });
        }

        var result = await _productService.GetAllAsync();

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _productService.GetByIdAsync(id);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
    }

    /// <summary>
    /// Create new product
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        if (_tenantProvider.TenantId == null)
        {
            return BadRequest(new { message = "Tenant not found" });
        }

        var result = await _productService.CreateAsync(dto);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>
    /// Update product
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var result = await _productService.UpdateAsync(id, dto);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return NoContent();
    }

    /// <summary>
    /// Delete product (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _productService.DeleteAsync(id);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return NoContent();
    }
}