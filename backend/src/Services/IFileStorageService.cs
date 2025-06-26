namespace OCR.Api.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file);
    Task DeleteFileAsync(string filePath);
    string GetFileUrl(string fileName);
}