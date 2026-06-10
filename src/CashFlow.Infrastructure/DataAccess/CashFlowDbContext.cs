using CashFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CashFlow.Infrastructure.DataAccess;

internal class CashFlowDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Expense> Expenses { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Expense>()
            .Property(e => e.Title)
            .HasMaxLength(50);

        modelBuilder.Entity<Expense>()
            .Property(e => e.Description)
            .HasMaxLength(1000);

        modelBuilder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasPrecision(10, 2);
    }
}
