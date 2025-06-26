using Azure;
using Azure.AI.DocumentIntelligence;
using OCR.Api.Models;

namespace OCR.Api.Services;

public class OcrService : IOcrService
{
    private readonly DocumentIntelligenceClient _client;
    private readonly ILogger<OcrService> _logger;

    public OcrService(IConfiguration configuration, ILogger<OcrService> logger)
    {
         var endpoint = configuration["Azure:FormRecognizer:Endpoint"] 
            ?? throw new ArgumentNullException("Azure:FormRecognizer:Endpoint configuration is missing");
        var apiKey = configuration["Azure:FormRecognizer:ApiKey"]
            ?? throw new ArgumentNullException("Azure:FormRecognizer:ApiKey configuration is missing");

        Console.WriteLine($"Using Azure Document Intelligence endpoint: {endpoint}");
          
        _client = new DocumentIntelligenceClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        _logger = logger;
    }

    public async Task<Receipt> ParseReceiptAsync(Stream fileStream)
    {
        if (fileStream == null || fileStream.Length == 0)
        {
            throw new ArgumentException("File stream cannot be null or empty", nameof(fileStream));
        }

        try
        {
            var content = new AnalyzeDocumentContent
            {
                Base64Source = BinaryData.FromStream(fileStream)
            };

            Operation<AnalyzeResult> operation = await _client.AnalyzeDocumentAsync(
                WaitUntil.Completed, "prebuilt-receipt", test);

                Receipt receipt = ExtractReceiptData(operation.Value);

            return receipt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing receipt");
            throw;
        }
    }

    private Receipt ExtractReceiptData(AnalyzeResult result)
    {
        if (!result.Documents.Any())
        {
            throw new InvalidOperationException("No receipt found in the document");
        }

        var document = result.Documents[0];
        var receipt = new Receipt();

        try
        {
            // Extract merchant name
            if (document.Fields.TryGetValue("MerchantName", out var merchantField))
            {
                receipt.VendorName = merchantField.ValueString ?? string.Empty;
            }

            // Extract receipt date
            if (document.Fields.TryGetValue("TransactionDate", out var dateField))
            {
                receipt.ReceiptDate = dateField.ValueDate?.Date;
            }

            // Extract total amount
            if (document.Fields.TryGetValue("Total", out var totalField))
            {
                receipt.TotalAmount = totalField.ValueCurrency.Amount;
            }

            // Extract tax amount
            if (document.Fields.TryGetValue("TotalTax", out var taxField))
            {
                receipt.TaxAmount = taxField.ValueCurrency.Amount;
            }

            // Extract tip amount
            if (document.Fields.TryGetValue("Tip", out var tipField))
            {
                receipt.TipAmount = tipField.ValueCurrency.Amount;
            }

            // Extract items
            // if (document.Fields.TryGetValue("Items", out var itemsField) && 
            //     itemsField.Content != null)
            // {
            //     var items = itemsField.Content.AsList();
            //     foreach (var itemField in items)
            //     {
            //         if (itemField == null) continue;

            //         var receiptItem = new ReceiptItem
            //         {
            //             Name = ExtractFieldValue(itemField, "Description", string.Empty),
            //             Quantity = (int)ExtractFieldValue(itemField, "Quantity", 1.0),
            //             UnitPrice = Convert.ToDecimal(ExtractFieldValue(itemField, "Price", 0.0)),
            //             Total = Convert.ToDecimal(ExtractFieldValue(itemField, "TotalPrice", 0.0))
            //         };
            //         receipt.Items.Add(receiptItem);
            //     }
            // }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting receipt data from analysis result");
            throw;
        }

        return receipt;
    }

    // private T ExtractFieldValue<T>(DocumentField field, string fieldName, T defaultValue)
    // {
    //     try
    //     {
    //         if (field.c != null)
    //         {
    //             var dict = field.Value.AsDictionary();
    //             if (dict.TryGetValue(fieldName, out var value) && value.Value != null)
    //             {
    //                 if (typeof(T) == typeof(string))
    //                 {
    //                     return (T)(object)(value.Value.AsString() ?? string.Empty);
    //                 }
    //                 else if (typeof(T) == typeof(double))
    //                 {
    //                     return (T)(object)value.Value.AsDouble();
    //                 }
    //                 else if (typeof(T) == typeof(int))
    //                 {
    //                     return (T)(object)(int)value.Value.AsDouble();
    //                 }
    //             }
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogWarning(ex, "Error extracting field {FieldName}", fieldName);
    //     }
    //     return defaultValue;
    // }
}