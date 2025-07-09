using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Console.Choreography;

public class EventBus : IEventBus
{
    private readonly ILogger<EventBus> _logger;
    private readonly ConcurrentDictionary<string, List<Func<ISwarmEvent, CancellationToken, Task>>> _handlers = new();
    private readonly ConcurrentDictionary<string, List<ISwarmEvent>> _eventHistory = new();
    private readonly ConcurrentDictionary<string, EventSubscription> _subscriptions = new();

    public EventBus(ILogger<EventBus> logger)
    {
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : ISwarmEvent
    {
        var eventType = typeof(T).Name;
        _logger.LogDebug("Publishing event {EventType} from agent {AgentId}", eventType, @event.SourceAgentId);

        StoreEvent(@event);

        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            var tasks = handlers.Select(handler => SafeHandleAsync(handler, @event, cancellationToken)).ToList();
            await Task.WhenAll(tasks);
        }

        await Task.CompletedTask;
    }

    public async Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : ISwarmEvent
    {
        var eventType = typeof(T).Name;
        
        var genericHandler = new Func<ISwarmEvent, CancellationToken, Task>(async (@event, ct) =>
        {
            if (@event is T typedEvent)
            {
                await handler(typedEvent, ct);
            }
        });

        var handlers = _handlers.GetOrAdd(eventType, _ => new List<Func<ISwarmEvent, CancellationToken, Task>>());
        
        lock (handlers)
        {
            handlers.Add(genericHandler);
        }

        var subscription = new EventSubscription
        {
            SubscriptionId = Guid.NewGuid().ToString(),
            EventType = eventType,
            AgentId = "system",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _subscriptions.TryAdd(subscription.SubscriptionId, subscription);

        _logger.LogDebug("Subscribed to event type {EventType}", eventType);
        await Task.CompletedTask;
    }

    public async Task UnsubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : ISwarmEvent
    {
        var eventType = typeof(T).Name;
        
        if (_handlers.TryGetValue(eventType, out var handlers))
        {
            lock (handlers)
            {
                handlers.RemoveAll(h => h.Method == handler.Method);
            }
        }

        _logger.LogDebug("Unsubscribed from event type {EventType}", eventType);
        await Task.CompletedTask;
    }

    public async Task<List<ISwarmEvent>> GetEventsAsync(string agentId, CancellationToken cancellationToken = default)
    {
        if (_eventHistory.TryGetValue(agentId, out var events))
        {
            return await Task.FromResult(new List<ISwarmEvent>(events));
        }

        return await Task.FromResult(new List<ISwarmEvent>());
    }

    private void StoreEvent(ISwarmEvent @event)
    {
        var agentEvents = _eventHistory.GetOrAdd("global", _ => new List<ISwarmEvent>());
        
        lock (agentEvents)
        {
            agentEvents.Add(@event);
            
            if (agentEvents.Count > 1000)
            {
                agentEvents.RemoveRange(0, 100);
            }
        }

        if (!string.IsNullOrEmpty(@event.SourceAgentId))
        {
            var sourceEvents = _eventHistory.GetOrAdd(@event.SourceAgentId, _ => new List<ISwarmEvent>());
            
            lock (sourceEvents)
            {
                sourceEvents.Add(@event);
                
                if (sourceEvents.Count > 100)
                {
                    sourceEvents.RemoveRange(0, 10);
                }
            }
        }
    }

    private async Task SafeHandleAsync(Func<ISwarmEvent, CancellationToken, Task> handler, ISwarmEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            await handler(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling event {EventType} from agent {AgentId}", 
                @event.EventType, @event.SourceAgentId);
        }
    }

    private class EventSubscription : IEventSubscription
    {
        public string SubscriptionId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string AgentId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}