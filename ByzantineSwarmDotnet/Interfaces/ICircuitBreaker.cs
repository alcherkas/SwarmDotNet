namespace ByzantineSwarmDotnet.Interfaces;

public interface ICircuitBreaker
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation);
    Task ExecuteAsync(Func<Task> operation);
    bool IsOpen { get; }
    bool IsClosed { get; }
    bool IsHalfOpen { get; }
}