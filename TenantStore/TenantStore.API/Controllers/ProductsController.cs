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
    private readonly IFileService _fileService;

    public ProductsController(IProductService productService, ITenantProvider tenantProvider, IFileService fileService)
    {
        _productService = productService;
        _tenantProvider = tenantProvider;
        _fileService = fileService;
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
    [Authorize(Roles = "Admin,SuperAdmin")] // User cannot delete
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _productService.DeleteAsync(id);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return NoContent();
    }

    /// <summary>
/// Upload product image
/// </summary>
[HttpPost("upload-image")]
[Authorize]
public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
{
    if (_tenantProvider.TenantId == null)
    {
        return BadRequest(new { message = "Tenant not found" });
    }

    if (file == null || file.Length == 0)
    {
        return BadRequest(new { message = "No file uploaded" });
    }

    if (!_fileService.IsValidImage(file))
    {
        return BadRequest(new { message = "Invalid file type. Only images (jpg, jpeg, png, gif, webp) are allowed." });
    }

    try
    {
        var imageUrl = await _fileService.UploadProductImageAsync(file, _tenantProvider.TenantId.Value);
        
        return Ok(new { 
            imageUrl = imageUrl,
            message = "Image uploaded successfully" 
        });
    }
    catch (Exception ex)
    {
        return BadRequest(new { message = ex.Message });
    }
}
}