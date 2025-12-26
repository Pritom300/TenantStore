namespace TenantStore.Infrastructure.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TenantStore.Application.Interfaces;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> UploadProductImageAsync(IFormFile file, Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty");

        if (!IsValidImage(file))
            throw new ArgumentException("Invalid file type. Only images are allowed.");

        if (file.Length > MaxFileSize)
            throw new ArgumentException("File size exceeds 5MB limit");

        // Create unique filename
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        var uniqueFileName = $"{tenantId}_{Guid.NewGuid()}{fileExtension}";

        // Create directory path
        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");
        Directory.CreateDirectory(uploadsFolder); // Create if doesn't exist

        // Full file path
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        // Return relative URL
        return $"/uploads/products/{uniqueFileName}";
    }

    public async Task<bool> DeleteProductImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(imageUrl))
            return false;

        try
        {
            // Extract filename from URL
            var fileName = Path.GetFileName(imageUrl);
            var filePath = Path.Combine(_environment.WebRootPath, "uploads", "products", fileName);

            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath), cancellationToken);
                return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    public bool IsValidImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLower();
        return _allowedExtensions.Contains(extension);
    }
}