using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace APIGateway.Middleware
{
    /// <summary>
    /// Authentication middleware for validating JWT tokens
    /// </summary>
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _jwtSecret;

        public AuthenticationMiddleware(RequestDelegate next, string jwtSecret)
        {
            _next = next;
            _jwtSecret = jwtSecret;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader?.StartsWith("Bearer ") == true)
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    
                    // Validate token with Auth Service
                    var isValid = await ValidateTokenAsync(token);
                    if (isValid)
                    {
                        // Token is valid, proceed
                        context.Items["Token"] = token;
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
                        return;
                    }
                }
                else
                {
                    // Check if route requires authentication
                    var route = Routing.RouteTable.FindRoute(context.Request.Path);
                    if (route?.RequireAuthentication == true)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new { error = "Unauthorized" });
                        return;
                    }
                }

                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new { error = "Internal Server Error" });
            }
        }

        private async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5001/api/v1/auth/validate-token");
                    request.Content = new StringContent(
                        $"{{\"token\": \"{token}\"}}",
                        Encoding.UTF8,
                        "application/json");

                    var response = await client.SendAsync(request);
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Authorization middleware for role-based access control
    /// </summary>
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var route = Routing.RouteTable.FindRoute(context.Request.Path);
            
            if (route?.AllowedRoles.Any() == true)
            {
                var userRoles = context.Items["UserRoles"] as List<string>;
                if (userRoles == null || !userRoles.Intersect(route.AllowedRoles).Any())
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new { error = "Forbidden" });
                    return;
                }
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Rate limiting middleware
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Dictionary<string, RateLimitBucket> Buckets = new();

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var userId = context.Items["UserId"]?.ToString() ?? context.Connection.RemoteIpAddress?.ToString();
            
            if (!Buckets.ContainsKey(userId))
            {
                Buckets[userId] = new RateLimitBucket(100, TimeSpan.FromMinutes(1));
            }

            var bucket = Buckets[userId];
            if (!bucket.TryConsume())
            {
                context.Response.StatusCode = 429;
                context.Response.Headers["Retry-After"] = "60";
                await context.Response.WriteAsJsonAsync(new { error = "Too many requests" });
                return;
            }

            await _next(context);
        }

        private class RateLimitBucket
        {
            private int _tokens;
            private readonly int _capacity;
            private readonly TimeSpan _refillInterval;
            private DateTime _lastRefillTime = DateTime.UtcNow;

            public RateLimitBucket(int capacity, TimeSpan refillInterval)
            {
                _tokens = capacity;
                _capacity = capacity;
                _refillInterval = refillInterval;
            }

            public bool TryConsume()
            {
                Refill();
                
                if (_tokens > 0)
                {
                    _tokens--;
                    return true;
                }

                return false;
            }

            private void Refill()
            {
                var now = DateTime.UtcNow;
                if (now - _lastRefillTime >= _refillInterval)
                {
                    _tokens = _capacity;
                    _lastRefillTime = now;
                }
            }
        }
    }

    /// <summary>
    /// Correlation ID middleware for distributed tracing
    /// </summary>
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() 
                               ?? Guid.NewGuid().ToString();
            
            context.Items["CorrelationId"] = correlationId;
            context.Response.Headers["X-Correlation-ID"] = correlationId;

            await _next(context);
        }
    }

    /// <summary>
    /// Request logging middleware
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Items["CorrelationId"]?.ToString();
            
            Console.WriteLine($"[{correlationId}] {context.Request.Method} {context.Request.Path}");

            await _next(context);

            Console.WriteLine($"[{correlationId}] Response: {context.Response.StatusCode}");
        }
    }
}
