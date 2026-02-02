using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace APIGateway.Routing
{
    /// <summary>
    /// Service discovery and routing configuration
    /// </summary>
    public class ServiceRegistry
    {
        private static readonly Dictionary<string, ServiceEndpoint> Services = new()
        {
            { "auth", new ServiceEndpoint { Name = "Auth Service", Url = "http://localhost:5001", Timeout = 10000 } },
            { "menu", new ServiceEndpoint { Name = "Menu Service", Url = "http://localhost:5002", Timeout = 10000 } },
            { "inventory", new ServiceEndpoint { Name = "Inventory Service", Url = "http://localhost:5003", Timeout = 10000 } },
            { "order", new ServiceEndpoint { Name = "Order Service", Url = "http://localhost:5004", Timeout = 10000 } },
            { "payment", new ServiceEndpoint { Name = "Payment Service", Url = "http://localhost:5005", Timeout = 30000 } },
            { "kds", new ServiceEndpoint { Name = "Kitchen Display Service", Url = "http://localhost:5006", Timeout = 10000 } },
            { "loyalty", new ServiceEndpoint { Name = "Loyalty Service", Url = "http://localhost:5007", Timeout = 10000 } },
            { "notification", new ServiceEndpoint { Name = "Notification Service", Url = "http://localhost:5008", Timeout = 10000 } }
        };

        public static ServiceEndpoint GetService(string serviceName)
        {
            return Services.TryGetValue(serviceName.ToLower(), out var service) ? service : null;
        }

        public static string ResolveUrl(string path)
        {
            // Parse path: /api/v1/{service}/{rest}
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            if (segments.Length < 3)
            {
                return null;
            }

            var serviceName = segments[2];
            var service = GetService(serviceName);
            
            if (service == null)
            {
                return null;
            }

            // Reconstruct path without service name
            var remainingPath = "/" + string.Join("/", segments.Skip(3));
            return $"{service.Url}{remainingPath}";
        }
    }

    /// <summary>
    /// Service endpoint configuration
    /// </summary>
    public class ServiceEndpoint
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public int Timeout { get; set; }
    }

    /// <summary>
    /// Route configuration for API Gateway
    /// </summary>
    public class RouteConfig
    {
        public string Pattern { get; set; }
        public string Service { get; set; }
        public List<string> AllowedRoles { get; set; } = new();
        public bool RequireAuthentication { get; set; } = true;
    }

    /// <summary>
    /// Route table for API Gateway
    /// </summary>
    public static class RouteTable
    {
        public static List<RouteConfig> GetRoutes()
        {
            return new List<RouteConfig>
            {
                // Auth endpoints (no auth required)
                new RouteConfig { Pattern = "/api/v1/auth/login", Service = "auth", RequireAuthentication = false },
                new RouteConfig { Pattern = "/api/v1/auth/register", Service = "auth", RequireAuthentication = false },
                new RouteConfig { Pattern = "/api/v1/auth/validate-token", Service = "auth", RequireAuthentication = false },
                new RouteConfig { Pattern = "/api/v1/auth/refresh-token", Service = "auth", RequireAuthentication = false },

                // Menu endpoints
                new RouteConfig { Pattern = "/api/v1/menu/*", Service = "menu", AllowedRoles = new List<string> { "customer", "staff", "admin" } },

                // Order endpoints
                new RouteConfig { Pattern = "/api/v1/orders/*", Service = "order", AllowedRoles = new List<string> { "customer", "staff", "admin" } },

                // Inventory endpoints (staff/admin only)
                new RouteConfig { Pattern = "/api/v1/inventory/*", Service = "inventory", AllowedRoles = new List<string> { "staff", "admin" } },

                // Payment endpoints
                new RouteConfig { Pattern = "/api/v1/payments/*", Service = "payment", AllowedRoles = new List<string> { "customer", "staff", "admin" } },

                // KDS endpoints (staff/admin only)
                new RouteConfig { Pattern = "/api/v1/kds/*", Service = "kds", AllowedRoles = new List<string> { "staff", "admin" } },

                // Loyalty endpoints
                new RouteConfig { Pattern = "/api/v1/loyalty/*", Service = "loyalty", AllowedRoles = new List<string> { "customer", "admin" } },

                // Notification endpoints (admin only)
                new RouteConfig { Pattern = "/api/v1/notifications/*", Service = "notification", AllowedRoles = new List<string> { "admin" } }
            };
        }

        public static RouteConfig FindRoute(string path)
        {
            return GetRoutes().FirstOrDefault(r => 
                path.StartsWith(r.Pattern.TrimEnd('*')));
        }
    }
}
