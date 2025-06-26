using OCR.Api.Models;

namespace OCR.Api.Services;

public interface IOcrService
{
  Task<Receipt> ParseReceiptAsync(Stream fileStream);
}