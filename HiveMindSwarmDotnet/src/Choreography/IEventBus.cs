namespace HiveMindSwarmDotnet.Console.Choreography;

public interface IEventBus
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : ISwarmEvent;
    Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : ISwarmEvent;
    Task UnsubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : ISwarmEvent;
    Task<List<ISwarmEvent>> GetEventsAsync(string agentId, CancellationToken cancellationToken = default);
}

public interface IEventHandler<in T> where T : ISwarmEvent
{
    Task HandleAsync(T @event, CancellationToken cancellationToken = default);
}

public interface IEventSubscription
{
    string SubscriptionId { get; }
    string EventType { get; }
    string AgentId { get; }
    DateTime CreatedAt { get; }
    bool IsActive { get; set; }
}