# OCR Receipt Application – Specification Document

## 1. Overview

The OCR Receipt Application is a web application enables users to upload receipts. The system extracts structured data from the uploaded image (e.g., vendor name, total amount, tax, tip, and line items) using Azure Document Intelligence. The extracted details are automatically prefilled in a the form for review and submission.

## 2. Functional Specifications

### 2.1 User Stories

- **As a user**, I want to upload a receipt image so that its data is read and filled into a form.
- **As a user**, I want to submit the expense.

### 2.2 Features

- Receipt upload (image/PDF support)
- Client-side form with the following fields, prefilled by OCR:
  - Vendor Name
  - Receipt Date
  - Total Amount
  - Tax Amount
  - Tip Amount
  - Line Items (name, quantity, price per item, total for item)
- Manual edit/correction before final submission
- Validation for all required fields
- Success/Error notifications
- Storage of original receipt and extracted data

### 2.3 User Flow

1. User navigates to the upload page.
2. User uploads a receipt (image/PDF).
3. Receipt is sent to backend API.
4. Backend processes the receipt via Azure Document Intelligence, parses result.
5. API responds with structured data.
6. Frontend form is prefilled for user review.
7. User reviews/edits and submits final data.
8. Data is stored in the backend.

## 3. Technical Specifications

### 3.1 Frontend (Angular)

- Angular (v14+ recommended)
- File Upload Component (input + drag/drop support)
- Dynamic Form for parsed receipt details
- Reactive forms for validation and editing
- Service to communicate with backend API (HTTP Client)
- UI feedback for progress, errors, and completion

#### Endpoints Consumed

- `POST /api/receipts/parse` – Uploads file and receives parsed details
- `POST /api/receipts` – Saves reviewed/edited receipt data

#### Sample Data Model (TypeScript)

```typescript
export interface ReceiptItem {
  name: string;
  quantity: number;
  unitPrice: number;
  total: number;
}

export interface ReceiptData {
  vendorName: string;
  receiptDate: string;
  totalAmount: number;
  taxAmount: number;
  tipAmount?: number;
  items: ReceiptItem[];
}
```

### 3.2 Backend (.NET Core WebAPI)

- ASP.NET Core Web API (v6+ recommended)
- Endpoints for:
  - Receipt file upload and OCR parsing
  - Receipt data storage
- Integration with Azure Document Intelligence SDK
- Secure API key management (do not hard-code keys)
- Error handling and validation

#### Key Technologies

- Azure.AI.DocumentIntelligence
- .NET Web API
- Angular 19 for Front end application 

#### Sample Controller Code

```csharp
[ApiController]
[Route("api/receipts")]
public class ReceiptsController : ControllerBase
{
    [HttpPost("parse")]
    public async Task<IActionResult> ParseReceipt([FromForm] IFormFile file)
    {
        // Upload file to storage, get URI or stream
        // Call Azure Document Intelligence API as per sample code provided
        // Parse fields and return structured ReceiptData as JSON
    }

    [HttpPost]
    public async Task<IActionResult> SaveReceipt([FromBody] ReceiptData data)
    {
        // Save final, user-reviewed receipt data to database
    }
}
```

#### Azure Document Intelligence Integration Sample

```csharp
using Azure;
using Azure.AI.DocumentIntelligence;

string endpoint = "YOUR_FORM_RECOGNIZER_ENDPOINT";
string apiKey = "YOUR_FORM_RECOGNIZER_KEY";
AzureKeyCredential credential = new AzureKeyCredential(apiKey);
DocumentIntelligenceClient client = new DocumentIntelligenceClient(new Uri(endpoint), credential);

Uri receiptUri = new Uri("https://raw.githubusercontent.com/Azure/azure-sdk-for-python/main/sdk/formrecognizer/azure-ai-formrecognizer/tests/sample_forms/receipt/contoso-receipt.png");

Operation<AnalyzeResult> operation = await client.AnalyzeDocumentAsync(
    WaitUntil.Completed, "prebuilt-receipt", receiptUri
);

AnalyzeResult receipts = operation.Value;

foreach (AnalyzedDocument receipt in receipts.Documents)
{
    // Fetch the required details 
}
```

## 4. Architecture Diagram

```
[Angular Frontend]
      ↓
[.NET Core WebAPI]
      ↓
[Azure Document Intelligence]
      ↓
[Return Parsed Data]
      ↓
[Frontend Dynamic Form]
      ↓
[Save Data in Database]
```

*Note: A detailed architecture diagram can be found in `docs/architecture-diagram.png`*

## 5. Sample API Flow

### 5.1 Upload & Parse

**Endpoint:** `POST /api/receipts/parse`
- **Request:** Multipart/form-data (file)
- **Response:** JSON (parsed receipt fields)

### 5.2 Save Final Receipt

**Endpoint:** `POST /api/receipts`
- **Request:** JSON (user-reviewed receipt data)
- **Response:** 200 OK

# OCR Receipt Application – Specification Document
## 6. Folder Structure

```
ocr/
├── frontend/              # React TypeScript frontend
│   ├── src/
│   │   ├── components/    # Reusable UI components
│   │   ├── pages/        # Page-level components
│   │   ├── services/     # API service layer
│   │   ├── models/       # TypeScript interfaces
│   │   ├── utils/        # Utility functions
│   │   ├── styles/       # Global and component styles
│   │   ├── App.tsx       # Main component
│   │   └── index.tsx     # Entry point
│   ├── public/           # Static files
│   └── package.json      # Frontend dependencies
│
├── backend/              # .NET Core backend
│   ├── src/
│   │   ├── Controllers/  # API endpoints
│   │   ├── Models/       # Data models
│   │   ├── Services/     # Business logic
│   │   ├── Data/         # Database layer
│   │   ├── Middleware/   # Custom middleware
│   │   ├── Program.cs    # Entry point
│   │   └── Startup.cs    # Configuration
│   └── *.csproj         # Project file
│
├── docs/                # Documentation
├── tests/              # Test suites
└── README.md           # Project overview
```


## 6. Folder Structure

```
ocr/
├── frontend/              # React TypeScript frontend
│   ├── src/
│   │   ├── components/    # Reusable UI components
│   │   ├── pages/        # Page-level components
│   │   ├── services/     # API service layer
│   │   ├── models/       # TypeScript interfaces
│   │   ├── utils/        # Utility functions
│   │   ├── styles/       # Global and component styles
│   │   ├── App.tsx       # Main component
│   │   └── index.tsx     # Entry point
│   ├── public/           # Static files
│   └── package.json      # Frontend dependencies
│
├── backend/              # .NET Core backend
│   ├── src/
│   │   ├── Controllers/  # API endpoints
│   │   ├── Models/       # Data models
│   │   ├── Services/     # Business logic
│   │   ├── Data/         # Database layer
│   │   ├── Middleware/   # Custom middleware
│   │   ├── Program.cs    # Entry point
│   │   └── Startup.cs    # Configuration
│   └── *.csproj         # Project file
│
├── docs/                # Documentation
├── tests/              # Test suites
└── README.md           # Project overview
```
