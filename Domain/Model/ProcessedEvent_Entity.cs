namespace InventoryService.Domain.Model;

public class ProcessedEvent
{
    public Guid EventId { get; private set; }
    public DateTime ProcessedAt { get; private set; }


    protected ProcessedEvent() { }

    public ProcessedEvent(Guid eventId)
    {
        EventId = eventId;
        ProcessedAt = DateTime.UtcNow;
    }

}