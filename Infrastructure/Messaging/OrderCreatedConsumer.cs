using Confluent.Kafka;
using InventoryService.Domain.Model;
using InventoryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using InventoryService.Infrastructure.Persistence;

namespace InventoryService.Infrastructure.Messaging;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(
        IServiceScopeFactory scopeFactory,
        IConfiguration config,
        ILogger<OrderCreatedConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = _config["Kafka:BootstrapServers"],
            GroupId = _config["Kafka:GroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
        
        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
        consumer.Subscribe(_config["Kafka:OrderCreatedTopic"]);

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = consumer.Consume(stoppingToken);
            try
            {
                await HandleEvent(result.Message.Value);
                consumer.Commit(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message: {Payload}", result.Message.Value);
                // DO NOT COMMIT — Kafka will redeliver
            }
        }
    }

    private async Task HandleEvent(string payload)
    {
        
        _logger.LogInformation("Consumed Kafka payload: {Payload}", payload);
        var evt = JsonSerializer.Deserialize<OrderCreatedEvent>(payload,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }
            );
        _logger.LogInformation("Serialized Payload: {evt}", evt);
    
        if (evt == null)
        {
            _logger.LogWarning("Received invalid OrderCreatedEvent payload: {Payload}", payload);
            return;
        }
        
        if (evt.Items == null || evt.Items.Count == 0)
        {
            _logger.LogWarning("OrderCreatedEvent {EventId} has no items", evt.EventId);
            return;
        }
        
        using var scope = _scopeFactory.CreateScope();
        var db =  scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

        if (await db.ProcessedEvents.AnyAsync(e => e.EventId == evt.EventId))
            return;

        foreach (var item in evt.Items)
        {
            var stock = await db.StockItems.FindAsync(item.ProductId);
            if (stock == null || !stock.CanReserve(item.Quantity))
            {
                db.StockReservations.Add(
                    new StockReservation(evt.OrderId, item.ProductId, item.Quantity)
                );
            }
        }

        db.ProcessedEvents.Add(new ProcessedEvent(evt.EventId));
        await db.SaveChangesAsync();
    }

    
}

