using OCR.Api.Data;
using OCR.Api.Models;

namespace OCR.Api.Services;

public class ReceiptService : IReceiptService
{
    private readonly IReceiptRepository _repository;
    private readonly ILogger<ReceiptService> _logger;

    public ReceiptService(IReceiptRepository repository, ILogger<ReceiptService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<Receipt>> GetAllReceiptsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Receipt?> GetReceiptByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Receipt> CreateReceiptAsync(Receipt receipt)
    {
        receipt.CreatedAt = DateTime.UtcNow;
        return await _repository.CreateAsync(receipt);
    }

    public async Task<bool> DeleteReceiptAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<Receipt?> UpdateReceiptAsync(int id, Receipt receipt)
    {
        if (id != receipt.Id)
        {
            return null;
        }

        return await _repository.UpdateAsync(receipt);
    }
}