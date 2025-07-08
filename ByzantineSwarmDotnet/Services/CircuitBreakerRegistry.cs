using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using ByzantineSwarmDotnet.Interfaces;

namespace ByzantineSwarmDotnet.Services;

public class CircuitBreakerRegistry : ICircuitBreakerRegistry
{
    private readonly ILogger<CircuitBreakerRegistry> _logger;
    private readonly Dictionary<string, CircuitBreakerWrapper> _circuitBreakers;
    private readonly object _lockObject = new();

    public CircuitBreakerRegistry(ILogger<CircuitBreakerRegistry> logger)
    {
        _logger = logger;
        _circuitBreakers = new Dictionary<string, CircuitBreakerWrapper>();
    }

    public ICircuitBreaker GetOrCreate(string key)
    {
        lock (_lockObject)
        {
            if (!_circuitBreakers.TryGetValue(key, out var circuitBreaker))
            {
                circuitBreaker = new CircuitBreakerWrapper(key, _logger);
                _circuitBreakers[key] = circuitBreaker;
                _logger.LogInformation("Created circuit breaker for key: {Key}", key);
            }
            
            return circuitBreaker;
        }
    }

    public async Task<bool> IsOpenAsync(string key)
    {
        lock (_lockObject)
        {
            if (_circuitBreakers.TryGetValue(key, out var circuitBreaker))
            {
                return circuitBreaker.IsOpen;
            }
            return false;
        }
    }

    public async Task ResetAsync(string key)
    {
        lock (_lockObject)
        {
            if (_circuitBreakers.TryGetValue(key, out var circuitBreaker))
            {
                circuitBreaker.Reset();
                _logger.LogInformation("Reset circuit breaker for key: {Key}", key);
            }
        }
    }

    public async Task<Dictionary<string, string>> GetAllStatesAsync()
    {
        lock (_lockObject)
        {
            return _circuitBreakers.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.State
            );
        }
    }

    private class CircuitBreakerWrapper : ICircuitBreaker
    {
        private readonly string _key;
        private readonly ILogger _logger;
        private readonly ResiliencePipeline _pipeline;
        private CircuitBreakerState _state = CircuitBreakerState.Closed;
        private int _failureCount = 0;
        private DateTime _lastFailureTime = DateTime.MinValue;
        private readonly int _failureThreshold = 5;
        private readonly TimeSpan _timeout = TimeSpan.FromMinutes(1);

        public CircuitBreakerWrapper(string key, ILogger logger)
        {
            _key = key;
            _logger = logger;
            
            _pipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 3,
                    BreakDuration = TimeSpan.FromSeconds(30),
                    OnOpened = args =>
                    {
                        _state = CircuitBreakerState.Open;
                        _logger.LogWarning("Circuit breaker opened for key: {Key}", _key);
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = args =>
                    {
                        _state = CircuitBreakerState.Closed;
                        _logger.LogInformation("Circuit breaker closed for key: {Key}", _key);
                        return ValueTask.CompletedTask;
                    },
                    OnHalfOpened = args =>
                    {
                        _state = CircuitBreakerState.HalfOpen;
                        _logger.LogInformation("Circuit breaker half-opened for key: {Key}", _key);
                        return ValueTask.CompletedTask;
                    }
                })
                .Build();
        }

        public bool IsOpen => _state == CircuitBreakerState.Open;
        public bool IsClosed => _state == CircuitBreakerState.Closed;
        public bool IsHalfOpen => _state == CircuitBreakerState.HalfOpen;

        public string State => _state.ToString();

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            try
            {
                return await _pipeline.ExecuteAsync(async ct => await operation());
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning("Circuit breaker is open for key: {Key}. Operation rejected.", _key);
                throw new InvalidOperationException($"Circuit breaker is open for {_key}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operation failed for circuit breaker key: {Key}", _key);
                throw;
            }
        }

        public async Task ExecuteAsync(Func<Task> operation)
        {
            try
            {
                await _pipeline.ExecuteAsync(async ct => await operation());
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogWarning("Circuit breaker is open for key: {Key}. Operation rejected.", _key);
                throw new InvalidOperationException($"Circuit breaker is open for {_key}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operation failed for circuit breaker key: {Key}", _key);
                throw;
            }
        }

        public void Reset()
        {
            _failureCount = 0;
            _lastFailureTime = DateTime.MinValue;
            _state = CircuitBreakerState.Closed;
        }
    }

    private enum CircuitBreakerState
    {
        Closed,
        Open,
        HalfOpen
    }
}
