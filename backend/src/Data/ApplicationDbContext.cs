using Microsoft.EntityFrameworkCore;
using OCR.Api.Models;

namespace OCR.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Receipt> Receipts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Receipt>()
            .Property(r => r.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Receipt>()
            .Property(r => r.TaxAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Receipt>()
            .Property(r => r.TipAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Receipt>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<Receipt>()
            .OwnsMany(r => r.Items, builder =>
            {
                builder.Property(i => i.UnitPrice).HasPrecision(18, 2);
                builder.Property(i => i.Total).HasPrecision(18, 2);
            });

        base.OnModelCreating(modelBuilder);
    }
}