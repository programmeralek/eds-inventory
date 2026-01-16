namespace InventoryService.Domain.Model;

public class StockItem
{
    public string ProductId { get;private set; }
    public int AvailableQuantity { get;private set; }

    protected StockItem(){}

    public StockItem(string productId, int initialQuantity)
    {
        ProductId = productId;
        AvailableQuantity = initialQuantity;
    }

    public bool CanReserve(int quantity)
        => AvailableQuantity >= quantity;

    public void Reserve(int quantity)
    {
     if (!CanReserve(quantity))
        throw new InvalidOperationException("Insufficient Stock!");   
    
        AvailableQuantity -= quantity;
    }

    public void Release(int quantity)
    {
        AvailableQuantity += quantity;
    }
}