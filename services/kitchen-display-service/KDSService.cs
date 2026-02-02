using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KDSService.Services
{
    /// <summary>
    /// Kitchen Display System service for managing kitchen orders in real-time
    /// </summary>
    public interface IKDSService
    {
        Task<Models.KDSOrderResponse> CreateKDSOrderAsync(string orderId, string orderNumber, List<dynamic> items);
        Task UpdateOrderStatusAsync(string orderId, Models.KDSOrderStatus status);
        Task<List<Models.KDSOrderResponse>> GetActiveOrdersAsync();
        Task<Models.KDSOrderResponse> GetOrderAsync(string orderId);
        Task<Models.KDSMetrics> GetMetricsAsync();
        Task AssignStationAsync(string orderId, Models.Station station);
    }

    public class KDSService : IKDSService
    {
        private readonly IKDSRepository _kdsRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IWebSocketManager _webSocketManager;

        public KDSService(
            IKDSRepository kdsRepository,
            IEventPublisher eventPublisher,
            IWebSocketManager webSocketManager)
        {
            _kdsRepository = kdsRepository;
            _eventPublisher = eventPublisher;
            _webSocketManager = webSocketManager;
        }

        public async Task<Models.KDSOrderResponse> CreateKDSOrderAsync(
            string orderId, string orderNumber, List<dynamic> items)
        {
            // Auto-assign station based on items (simplified logic)
            var station = DetermineStation(items);

            var kdsOrder = new Models.KDSOrder
            {
                OrderId = orderId,
                OrderNumber = orderNumber,
                AssignedStation = station,
                Items = items.Select(item => new Models.KDSOrderItem
                {
                    ItemId = item.MenuItemId,
                    MenuItemName = item.MenuItemName,
                    Quantity = item.Quantity,
                    SpecialInstructions = item.SpecialInstructions ?? new List<string>()
                }).ToList(),
                EstimatedMinutes = CalculateEstimatedTime(items)
            };

            await _kdsRepository.CreateAsync(kdsOrder);

            // Broadcast to all connected WebSocket clients
            var response = MapToResponse(kdsOrder);
            await _webSocketManager.BroadcastAsync(new Models.KDSWebSocketMessage
            {
                MessageType = "order-received",
                Order = response
            });

            return response;
        }

        public async Task UpdateOrderStatusAsync(string orderId, Models.KDSOrderStatus status)
        {
            var kdsOrder = await _kdsRepository.GetByOrderIdAsync(orderId);
            if (kdsOrder == null)
            {
                throw new InvalidOperationException("KDS Order not found");
            }

            kdsOrder.Status = status;

            if (status == Models.KDSOrderStatus.PREPARING && kdsOrder.StartedAt == null)
            {
                kdsOrder.StartedAt = DateTime.UtcNow;
            }

            if (status == Models.KDSOrderStatus.READY)
            {
                kdsOrder.ReadyAt = DateTime.UtcNow;
            }

            await _kdsRepository.UpdateAsync(kdsOrder);

            // Publish order status changed event
            await _eventPublisher.PublishAsync(new POS.Shared.Models.EventMessage
            {
                EventType = status switch
                {
                    Models.KDSOrderStatus.PREPARING => "order.preparing.v1",
                    Models.KDSOrderStatus.READY => "order.ready.v1",
                    _ => null
                },
                Source = "KDSService",
                Payload = new Dictionary<string, object>
                {
                    { "OrderId", orderId },
                    { "Status", status.ToString() }
                }
            });

            // Broadcast WebSocket update to all clients
            var response = MapToResponse(kdsOrder);
            await _webSocketManager.BroadcastAsync(new Models.KDSWebSocketMessage
            {
                MessageType = "order-updated",
                Order = response
            });
        }

        public async Task<List<Models.KDSOrderResponse>> GetActiveOrdersAsync()
        {
            var orders = await _kdsRepository.GetActiveOrdersAsync();
            return orders.Select(MapToResponse).ToList();
        }

        public async Task<Models.KDSOrderResponse> GetOrderAsync(string orderId)
        {
            var order = await _kdsRepository.GetByOrderIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            return MapToResponse(order);
        }

        public async Task<Models.KDSMetrics> GetMetricsAsync()
        {
            var orders = await _kdsRepository.GetActiveOrdersAsync();
            var allOrders = await _kdsRepository.GetAllOrdersAsync();

            var metrics = new Models.KDSMetrics
            {
                TotalOrders = orders.Count,
                PreparingOrders = orders.Count(o => o.Status == Models.KDSOrderStatus.PREPARING),
                ReadyOrders = orders.Count(o => o.Status == Models.KDSOrderStatus.READY),
                AveragePrepTime = allOrders
                    .Where(o => o.StartedAt.HasValue && o.ReadyAt.HasValue)
                    .Average(o => (o.ReadyAt.Value - o.StartedAt.Value).TotalMinutes),
                OrdersOverTime = orders.Count(o => 
                    o.StartedAt.HasValue && 
                    (DateTime.UtcNow - o.StartedAt.Value).TotalMinutes > o.EstimatedMinutes),
                OrdersByStation = orders
                    .GroupBy(o => o.AssignedStation)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return metrics;
        }

        public async Task AssignStationAsync(string orderId, Models.Station station)
        {
            var kdsOrder = await _kdsRepository.GetByOrderIdAsync(orderId);
            if (kdsOrder == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            kdsOrder.AssignedStation = station;
            await _kdsRepository.UpdateAsync(kdsOrder);

            var response = MapToResponse(kdsOrder);
            await _webSocketManager.BroadcastAsync(new Models.KDSWebSocketMessage
            {
                MessageType = "order-updated",
                Order = response
            });
        }

        private Models.Station DetermineStation(List<dynamic> items)
        {
            // Simplified logic - in production, would query item metadata
            return Models.Station.GRILL;
        }

        private int CalculateEstimatedTime(List<dynamic> items)
        {
            // Simplified logic - in production, would query menu service for prep times
            return 15; // Default 15 minutes
        }

        private Models.KDSOrderResponse MapToResponse(Models.KDSOrder order)
        {
            var elapsedTime = "";
            if (order.StartedAt.HasValue)
            {
                var elapsed = DateTime.UtcNow - order.StartedAt.Value;
                elapsedTime = $"{(int)elapsed.TotalMinutes:D2}:{elapsed.Seconds:D2}";
            }

            return new Models.KDSOrderResponse
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                Items = order.Items.Select(x => new Models.KDSOrderItemResponse
                {
                    MenuItemName = x.MenuItemName,
                    Quantity = x.Quantity,
                    SpecialInstructions = x.SpecialInstructions,
                    IsCompleted = x.IsCompleted
                }).ToList(),
                AssignedStation = order.AssignedStation,
                Status = order.Status,
                ReceivedAt = order.ReceivedAt,
                StartedAt = order.StartedAt,
                ReadyAt = order.ReadyAt,
                EstimatedMinutes = order.EstimatedMinutes,
                Priority = order.Priority,
                ElapsedTime = elapsedTime
            };
        }
    }

    /// <summary>
    /// Repository interface for KDS order persistence
    /// </summary>
    public interface IKDSRepository
    {
        Task<Models.KDSOrder> GetByOrderIdAsync(string orderId);
        Task<List<Models.KDSOrder>> GetActiveOrdersAsync();
        Task<List<Models.KDSOrder>> GetAllOrdersAsync();
        Task CreateAsync(Models.KDSOrder order);
        Task UpdateAsync(Models.KDSOrder order);
    }

    /// <summary>
    /// Event publisher interface
    /// </summary>
    public interface IEventPublisher
    {
        Task PublishAsync(POS.Shared.Models.EventMessage eventMessage);
    }

    /// <summary>
    /// WebSocket manager for real-time updates
    /// </summary>
    public interface IWebSocketManager
    {
        Task BroadcastAsync(Models.KDSWebSocketMessage message);
        Task SendToClientAsync(string clientId, Models.KDSWebSocketMessage message);
        Task RegisterClientAsync(string clientId);
        Task UnregisterClientAsync(string clientId);
    }
}
