namespace OCR.Api.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _uploadsPath;
    private readonly string _baseUrl;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IWebHostEnvironment environment, IConfiguration configuration, ILogger<FileStorageService> logger)
    {
        _uploadsPath = Path.Combine(environment.ContentRootPath, "uploads");
        _baseUrl = configuration["BaseUrl"] ?? "http://localhost:5215";
        _logger = logger;

        // Ensure uploads directory exists
        Directory.CreateDirectory(_uploadsPath);
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        try
        {
            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_uploadsPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving file {FileName}", file.FileName);
            throw;
        }
    }

    public async Task DeleteFileAsync(string fileName)
    {
        try
        {
            var filePath = Path.Combine(_uploadsPath, fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileName}", fileName);
            throw;
        }
    }

    public string GetFileUrl(string fileName)
    {
        return $"{_baseUrl}/uploads/{fileName}";
    }
}