using System;
using System.Collections.Generic;

namespace POS.Shared.Models
{
    /// <summary>
    /// Event message envelope for all domain events published to message broker
    /// </summary>
    public class EventMessage
    {
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        public string EventType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string CorrelationId { get; set; }
        public string Source { get; set; }
        public Dictionary<string, object> Payload { get; set; } = new();
    }

    /// <summary>
    /// Base API response envelope for standardized responses
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public Dictionary<string, string[]> Errors { get; set; } = new();

        public static ApiResponse<T> Ok(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        public static ApiResponse<T> Error(string message, Dictionary<string, string[]> errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new()
            };
        }
    }

    /// <summary>
    /// JWT token claims payload
    /// </summary>
    public class TokenClaims
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = new();
        public long IssuedAt { get; set; }
        public long ExpiresAt { get; set; }
    }

    /// <summary>
    /// Paging wrapper for list endpoints
    /// </summary>
    public class PaginatedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }

    /// <summary>
    /// Circuit breaker state tracking
    /// </summary>
    public enum CircuitState
    {
        Closed,
        Open,
        HalfOpen
    }

    /// <summary>
    /// Exception model for API error responses
    /// </summary>
    public class ErrorDetail
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string TraceId { get; set; }
    }
}
