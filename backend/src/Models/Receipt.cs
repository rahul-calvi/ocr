namespace OCR.Api.Models;

public class ReceiptItem
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}

public class Receipt
{
    public int Id { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public DateTime? ReceiptDate { get; set; }
    public double TotalAmount { get; set; }
    public double TaxAmount { get; set; }
    public double? TipAmount { get; set; }
    public List<ReceiptItem> Items { get; set; } = new();
    public string ImagePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}