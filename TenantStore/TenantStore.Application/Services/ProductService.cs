namespace TenantStore.Application.Services;

using TenantStore.Application.Common;
using TenantStore.Application.DTOs.Product;
using TenantStore.Application.Interfaces;
using TenantStore.Domain.Entities;
using TenantStore.Domain.Interfaces;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantProvider _tenantProvider;
    private readonly IFileService _fileService;

    public ProductService(IUnitOfWork unitOfWork, ITenantProvider tenantProvider, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _tenantProvider = tenantProvider;
        _fileService = fileService;
    }

    public async Task<Result<ProductDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return Result<ProductDto>.Failure("Product not found");

        return Result<ProductDto>.Success(MapToDto(product));
    }

    public async Task<Result<List<ProductDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (_tenantProvider.TenantId == null)
            return Result<List<ProductDto>>.Failure("Tenant not found");

        var products = await _unitOfWork.Products.GetProductsByTenantAsync(_tenantProvider.TenantId.Value, cancellationToken);
        var dtos = products.Select(MapToDto).ToList();
        return Result<List<ProductDto>>.Success(dtos);
    }

    //public async Task<Result<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    //{
    //    if (_tenantProvider.TenantId == null)
    //        return Result<ProductDto>.Failure("Tenant not found");

    //    var tenant = await _unitOfWork.Tenants.GetByIdAsync(_tenantProvider.TenantId.Value, cancellationToken);
    //    if (tenant == null)
    //        return Result<ProductDto>.Failure("Tenant not found");

    //    if (!tenant.CanAddProduct())
    //        return Result<ProductDto>.Failure("Product limit reached for this subscription");

    //    var product = Product.Create(_tenantProvider.TenantId.Value, dto.Name, dto.Price, dto.Stock, dto.Description);
    //    await _unitOfWork.Products.AddAsync(product, cancellationToken);
    //    await _unitOfWork.SaveChangesAsync(cancellationToken);

    //    return Result<ProductDto>.Success(MapToDto(product));
    //}

    public async Task<Result<ProductDto>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        if (_tenantProvider.TenantId == null)
            return Result<ProductDto>.Failure("Tenant not found");

        // Get tenant info
        var tenant = await _unitOfWork.Tenants.GetByIdAsync(_tenantProvider.TenantId.Value, cancellationToken);
        if (tenant == null)
            return Result<ProductDto>.Failure("Tenant not found");

        // Count ACTIVE products (excluding deleted ones)
        var currentProductCount = await _unitOfWork.Products
            .CountAsync(p => p.TenantId == _tenantProvider.TenantId.Value && !p.IsDeleted, cancellationToken);

        //  CHECK LIMIT
        if (currentProductCount >= tenant.MaxProducts)
        {
            return Result<ProductDto>.Failure(
                $"Product limit reached! Your {tenant.SubscriptionPlan} plan allows {tenant.MaxProducts} products. " +
                $"You currently have {currentProductCount} products. Please upgrade your subscription to add more products."
            );
        }

        var product = Product.Create(_tenantProvider.TenantId.Value, dto.Name, dto.Price, dto.Stock, dto.Description,dto.ImageUrl);
        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProductDto>.Success(MapToDto(product));
    }

    //public async Task<Result> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    //{
    //    var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
    //    if (product == null)
    //        return Result.Failure("Product not found");

    //    product.UpdateDetails(dto.Name, dto.Description, dto.Price);
    //    product.UpdateStock(dto.Stock);

    //    _unitOfWork.Products.Update(product);
    //    await _unitOfWork.SaveChangesAsync(cancellationToken);

    //    return Result.Success();
    //}
    public async Task<Result> UpdateAsync(Guid id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return Result.Failure("Product not found");

        // If new image is provided and old image exists, delete old image
        if (!string.IsNullOrEmpty(dto.ImageUrl) &&
            !string.IsNullOrEmpty(product.ImageUrl) &&
            dto.ImageUrl != product.ImageUrl)
        {
            await _fileService.DeleteProductImageAsync(product.ImageUrl, cancellationToken);
        }

        product.UpdateDetails(dto.Name, dto.Description, dto.Price);
        product.UpdateStock(dto.Stock);

        if (!string.IsNullOrEmpty(dto.ImageUrl))
        {
            product.SetImage(dto.ImageUrl);
        }

        _unitOfWork.Products.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    //public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    //{
    //    var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
    //    if (product == null)
    //        return Result.Failure("Product not found");

    //    // HARD DELETE - Actually remove from database
    //    _unitOfWork.Products.Remove(product);
    //    await _unitOfWork.SaveChangesAsync(cancellationToken);

    //    return Result.Success();
    //}
    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return Result.Failure("Product not found");

        // Delete product image if exists
        if (!string.IsNullOrEmpty(product.ImageUrl))
        {
            await _fileService.DeleteProductImageAsync(product.ImageUrl, cancellationToken);
        }

        _unitOfWork.Products.Remove(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }


    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
    }
}