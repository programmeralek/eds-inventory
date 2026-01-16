using InventoryService.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Persistence;

public class InventoryDbContext: DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options) {}

    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<StockReservation> StockReservations => Set<StockReservation>();
    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StockItem>()
            .HasKey(s => s.ProductId);
        modelBuilder.Entity<StockReservation>()
            .HasKey(r => r.Id);
        modelBuilder.Entity<ProcessedEvent>()
            .HasKey(e => e.EventId);
    }
    
}