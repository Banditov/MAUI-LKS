using MAUILKSAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MAUILKSAPI.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Sale> Sales { get; set; }
    public DbSet<User> Users { get; set; }
}