namespace TenantStore.Application.Interfaces;

using Microsoft.AspNetCore.Http;

public interface IFileService
{
    Task<string> UploadProductImageAsync(IFormFile file, Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteProductImageAsync(string imageUrl, CancellationToken cancellationToken = default);
    bool IsValidImage(IFormFile file);
}