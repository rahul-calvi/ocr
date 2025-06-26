using OCR.Api.Models;

namespace OCR.Api.Services;

public interface IReceiptService
{
    Task<IEnumerable<Receipt>> GetAllReceiptsAsync();
    Task<Receipt?> GetReceiptByIdAsync(int id);
    Task<Receipt> CreateReceiptAsync(Receipt receipt);
    Task<bool> DeleteReceiptAsync(int id);
    Task<Receipt?> UpdateReceiptAsync(int id, Receipt receipt);
}