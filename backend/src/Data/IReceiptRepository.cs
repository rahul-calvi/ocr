using OCR.Api.Models;

namespace OCR.Api.Data;

public interface IReceiptRepository
{
    Task<IEnumerable<Receipt>> GetAllAsync();
    Task<Receipt?> GetByIdAsync(int id);
    Task<Receipt> CreateAsync(Receipt receipt);
    Task<Receipt?> UpdateAsync(Receipt receipt);
    Task<bool> DeleteAsync(int id);
    Task SaveChangesAsync();
}