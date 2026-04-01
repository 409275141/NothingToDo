using Microsoft.EntityFrameworkCore;
using SimpleBookkeeping.Api.Models;

namespace SimpleBookkeeping.Api.Data;

public class SqliteDbContext : DbContext
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<CreditRecord> Credits => Set<CreditRecord>();

    public SqliteDbContext(DbContextOptions<SqliteDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.UserId).IsRequired();
            entity.Property(t => t.Amount).IsRequired();
            entity.Property(t => t.Type).IsRequired();
            entity.Property(t => t.Category).IsRequired();
            entity.Property(t => t.Date).IsRequired();
        });

        modelBuilder.Entity<CreditRecord>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.UserId).IsRequired();
            entity.Property(c => c.CustomerName).IsRequired();
            entity.Property(c => c.Amount).IsRequired();
            entity.Property(c => c.Status).IsRequired();
            entity.Property(c => c.Date).IsRequired();
            entity.Property(c => c.DueDate).IsRequired();
        });
    }
}
