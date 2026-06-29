using MAUILKSAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MAUILKSAPI.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Sale> Sales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Sale>()
            .Property(s => s.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Sale>().HasData(
            new Sale { Id = 1, ProductName = "Laptop", Price = 1200.00m, Sales = 45 },
            new Sale { Id = 2, ProductName = "Mouse", Price = 25.00m, Sales = 120 },
            new Sale { Id = 3, ProductName = "Keyboard", Price = 75.00m, Sales = 80 }
        );
    }
}