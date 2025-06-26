using Microsoft.EntityFrameworkCore;
using OCR.Api.Models;

namespace OCR.Api.Data;

public class ReceiptRepository : IReceiptRepository
{
    private readonly ApplicationDbContext _context;

    public ReceiptRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Receipt>> GetAllAsync()
    {
        return await _context.Receipts
            .Include(r => r.Items)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Receipt?> GetByIdAsync(int id)
    {
        return await _context.Receipts
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Receipt> CreateAsync(Receipt receipt)
    {
        _context.Receipts.Add(receipt);
        await SaveChangesAsync();
        return receipt;
    }

    public async Task<Receipt?> UpdateAsync(Receipt receipt)
    {
        var existingReceipt = await GetByIdAsync(receipt.Id);
        if (existingReceipt == null) return null;

        _context.Entry(existingReceipt).CurrentValues.SetValues(receipt);
        
        // Update items collection
        existingReceipt.Items.Clear();
        foreach (var item in receipt.Items)
        {
            existingReceipt.Items.Add(item);
        }

        await SaveChangesAsync();
        return existingReceipt;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var receipt = await _context.Receipts.FindAsync(id);
        if (receipt == null) return false;

        _context.Receipts.Remove(receipt);
        await SaveChangesAsync();
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}