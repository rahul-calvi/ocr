using OCR.Api.Services;
using OCR.Api.Middleware;
using OCR.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configure database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IOcrService, OcrService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientApp", policy =>
    {
        policy
            .WithOrigins(builder.Configuration["AllowedOrigins"]?.Split(',') ?? new[] { "http://localhost:3000" })
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Content-Disposition");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use custom exception handling middleware
app.UseExceptionHandling();

app.UseHttpsRedirection();
app.UseCors("ClientApp");

// Configure static file serving for uploaded files
var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseAuthorization();
app.MapControllers();

app.Run();
