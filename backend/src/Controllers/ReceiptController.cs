using Microsoft.AspNetCore.Mvc;
using OCR.Api.Models;
using OCR.Api.Services;

namespace OCR.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptController : ControllerBase
{
    private readonly IReceiptService _receiptService;
    private readonly IOcrService _ocrService;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<ReceiptController> _logger;

    public ReceiptController(
        IReceiptService receiptService,
        IOcrService ocrService,
        IFileStorageService fileStorageService,
        ILogger<ReceiptController> logger)
    {
        _receiptService = receiptService;
        _ocrService = ocrService;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Receipt>>> GetReceipts()
    {
        var receipts = await _receiptService.GetAllReceiptsAsync();
        return Ok(receipts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Receipt>> GetReceipt(int id)
    {
        var receipt = await _receiptService.GetReceiptByIdAsync(id);
        if (receipt == null)
        {
            return NotFound();
        }
        return Ok(receipt);
    }

    [HttpPost("parse")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
    public async Task<ActionResult<Receipt>> ParseReceipt(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "application/pdf" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
        {
            return BadRequest("Invalid file type. Only JPEG, PNG and PDF files are allowed.");
        }

        try
        {
            // Process the receipt with OCR
            Receipt parsedReceipt;
            using (var stream = file.OpenReadStream())
            {
                parsedReceipt = await _ocrService.ParseReceiptAsync(stream);
            }

            // Save the file
            return parsedReceipt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing receipt");
            return StatusCode(500, "Error processing receipt. Please try again.");
        }
    }

    [HttpPost]
    public async Task<ActionResult<Receipt>> CreateReceipt(Receipt receipt)
    {
        var createdReceipt = await _receiptService.CreateReceiptAsync(receipt);
        return CreatedAtAction(nameof(GetReceipt), new { id = createdReceipt.Id }, createdReceipt);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReceipt(int id, Receipt receipt)
    {
        var updatedReceipt = await _receiptService.UpdateReceiptAsync(id, receipt);
        if (updatedReceipt == null)
        {
            return NotFound();
        }
        return Ok(updatedReceipt);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReceipt(int id)
    {
        var receipt = await _receiptService.GetReceiptByIdAsync(id);
        if (receipt == null)
        {
            return NotFound();
        }

        // Extract filename from ImagePath
        var fileName = Path.GetFileName(receipt.ImagePath);
        if (!string.IsNullOrEmpty(fileName))
        {
            await _fileStorageService.DeleteFileAsync(fileName);
        }

        await _receiptService.DeleteReceiptAsync(id);
        return NoContent();
    }

    [HttpPost("test-ocr")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB limit
    public async Task<ActionResult<Receipt>> TestOcr(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "application/pdf" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
        {
            return BadRequest("Invalid file type. Only JPEG, PNG and PDF files are allowed.");
        }

        try
        {
            // Process the receipt with OCR only
            Receipt parsedReceipt;
            using (var stream = file.OpenReadStream())
            {
                parsedReceipt = await _ocrService.ParseReceiptAsync(stream);
            }

            return Ok(parsedReceipt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing receipt in test endpoint");
            return StatusCode(500, new { error = ex.Message, details = ex.ToString() });
        }
    }
}