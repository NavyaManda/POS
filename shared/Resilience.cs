using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace POS.Shared.Resilience
{
    /// <summary>
    /// Circuit breaker implementation for resilient HTTP calls between services
    /// States: Closed (normal) -> Open (failing) -> HalfOpen (recovery) -> Closed
    /// </summary>
    public class CircuitBreaker
    {
        private readonly int _failureThreshold;
        private readonly int _successThreshold;
        private readonly TimeSpan _timeout;
        private int _failureCount = 0;
        private int _successCount = 0;
        private DateTime _lastFailureTime = DateTime.MinValue;

        public Models.CircuitState State { get; private set; } = Models.CircuitState.Closed;

        public CircuitBreaker(int failureThreshold = 5, int successThreshold = 2, int timeoutSeconds = 30)
        {
            _failureThreshold = failureThreshold;
            _successThreshold = successThreshold;
            _timeout = TimeSpan.FromSeconds(timeoutSeconds);
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, string operationName = "")
        {
            if (State == Models.CircuitState.Open)
            {
                if (DateTime.UtcNow - _lastFailureTime > _timeout)
                {
                    State = Models.CircuitState.HalfOpen;
                    _successCount = 0;
                }
                else
                {
                    throw new InvalidOperationException($"Circuit breaker is OPEN for {operationName}. Service temporarily unavailable.");
                }
            }

            try
            {
                var result = await action();
                
                if (State == Models.CircuitState.HalfOpen)
                {
                    _successCount++;
                    if (_successCount >= _successThreshold)
                    {
                        State = Models.CircuitState.Closed;
                        _failureCount = 0;
                        _successCount = 0;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _failureCount++;
                _lastFailureTime = DateTime.UtcNow;

                if (_failureCount >= _failureThreshold)
                {
                    State = Models.CircuitState.Open;
                }

                throw;
            }
        }

        public void Reset()
        {
            State = Models.CircuitState.Closed;
            _failureCount = 0;
            _successCount = 0;
        }
    }

    /// <summary>
    /// Retry policy with exponential backoff
    /// </summary>
    public class RetryPolicy
    {
        private readonly int _maxRetries;
        private readonly int _initialDelayMs;
        private readonly int _maxDelayMs;
        private readonly double _backoffMultiplier;

        public RetryPolicy(int maxRetries = 3, int initialDelayMs = 1000, int maxDelayMs = 8000, double backoffMultiplier = 2.0)
        {
            _maxRetries = maxRetries;
            _initialDelayMs = initialDelayMs;
            _maxDelayMs = maxDelayMs;
            _backoffMultiplier = backoffMultiplier;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, string operationName = "")
        {
            int delay = _initialDelayMs;
            Exception lastException = null;

            for (int attempt = 0; attempt <= _maxRetries; attempt++)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex) when (attempt < _maxRetries && IsRetryable(ex))
                {
                    lastException = ex;
                    
                    // Add jitter: ±10%
                    int jitter = (int)(delay * 0.1 * (new Random().NextDouble() - 0.5) * 2);
                    int actualDelay = Math.Min(delay + jitter, _maxDelayMs);
                    
                    await Task.Delay(actualDelay);
                    delay = Math.Min((int)(delay * _backoffMultiplier), _maxDelayMs);
                }
            }

            throw lastException ?? new InvalidOperationException($"Operation {operationName} failed after {_maxRetries} retries");
        }

        private bool IsRetryable(Exception ex)
        {
            // Retry on timeout, transient HTTP errors (408, 429, 5xx)
            return ex is TimeoutException || 
                   ex is HttpRequestException ||
                   (ex.InnerException is HttpRequestException);
        }
    }

    /// <summary>
    /// Bulkhead pattern for limiting concurrent requests
    /// </summary>
    public class BulkheadPolicy
    {
        private readonly int _maxConcurrentCalls;
        private int _currentCalls = 0;
        private readonly object _lockObj = new();

        public BulkheadPolicy(int maxConcurrentCalls = 10)
        {
            _maxConcurrentCalls = maxConcurrentCalls;
        }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
        {
            lock (_lockObj)
            {
                if (_currentCalls >= _maxConcurrentCalls)
                {
                    throw new InvalidOperationException($"Bulkhead limit reached. Max concurrent calls: {_maxConcurrentCalls}");
                }
                _currentCalls++;
            }

            try
            {
                return await action();
            }
            finally
            {
                lock (_lockObj)
                {
                    _currentCalls--;
                }
            }
        }
    }
}
