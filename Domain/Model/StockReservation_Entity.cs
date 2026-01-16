namespace InventoryService.Domain.Model;

public class StockReservation
{
    
    public Guid Id { get; private set; } 
    public long OrderId { get; private set; } 
    public string ProductId { get; private set; } 
    public int Quantity { get; private set; } 
    public ReservationStatus Status { get; private set; } 
    public DateTime CreatedAt { get; private set; } 

    protected StockReservation(){}

    public StockReservation(long orderId, string productId, int quantity)
    {
        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        Status = ReservationStatus.PENDING;
        CreatedAt = DateTime.UtcNow;
    }

    public void Confirm() => Status = ReservationStatus.CONFIRMED;
    public void Reject() => Status = ReservationStatus.REJECTED;
    public void Release() => Status = ReservationStatus.RELEASED;

}