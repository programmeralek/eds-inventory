namespace InventoryService.Infrastructure.Messaging;

public class OrderCreatedEvent
{
    public Guid EventId { get; set; }
    public long OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<Item> Items { get; set; } = new();
    public DateTime OccurredAt { get; set; }

    public class Item
    {
        public string ProductId { get; set; } = default!;
        public int Quantity { get; set; }
    }
}