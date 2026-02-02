using System;
using System.Collections.Generic;

namespace KDSService.Models
{
    /// <summary>
    /// Station enumeration (kitchen stations)
    /// </summary>
    public enum Station
    {
        GRILL,
        FRYER,
        PREP,
        SALAD,
        DRINKS,
        PASTRY
    }

    /// <summary>
    /// Kitchen display order status
    /// </summary>
    public enum KDSOrderStatus
    {
        RECEIVED,
        PREPARING,
        READY,
        SERVED,
        CANCELLED
    }

    /// <summary>
    /// Kitchen display order entity
    /// </summary>
    public class KDSOrder
    {
        public string KDSOrderId { get; set; } = Guid.NewGuid().ToString();
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public List<KDSOrderItem> Items { get; set; } = new();
        public Station AssignedStation { get; set; }
        public KDSOrderStatus Status { get; set; } = KDSOrderStatus.RECEIVED;
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartedAt { get; set; }
        public DateTime? ReadyAt { get; set; }
        public int EstimatedMinutes { get; set; }
        public string Notes { get; set; }
        public int Priority { get; set; } = 0; // 0 = normal, 1 = high, -1 = low
    }

    /// <summary>
    /// Kitchen display order item
    /// </summary>
    public class KDSOrderItem
    {
        public string ItemId { get; set; }
        public string MenuItemName { get; set; }
        public int Quantity { get; set; }
        public List<string> SpecialInstructions { get; set; } = new();
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// KDS order response DTO
    /// </summary>
    public class KDSOrderResponse
    {
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public List<KDSOrderItemResponse> Items { get; set; }
        public Station AssignedStation { get; set; }
        public KDSOrderStatus Status { get; set; }
        public DateTime ReceivedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? ReadyAt { get; set; }
        public int EstimatedMinutes { get; set; }
        public int Priority { get; set; }
        public string ElapsedTime { get; set; } // MM:SS format
    }

    /// <summary>
    /// KDS order item response DTO
    /// </summary>
    public class KDSOrderItemResponse
    {
        public string MenuItemName { get; set; }
        public int Quantity { get; set; }
        public List<string> SpecialInstructions { get; set; }
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Update order status request
    /// </summary>
    public class UpdateKDSOrderStatusRequest
    {
        public string OrderId { get; set; }
        public KDSOrderStatus Status { get; set; }
    }

    /// <summary>
    /// KDS metrics response
    /// </summary>
    public class KDSMetrics
    {
        public int TotalOrders { get; set; }
        public int PreparingOrders { get; set; }
        public int ReadyOrders { get; set; }
        public double AveragePrepTime { get; set; }
        public int OrdersOverTime { get; set; } // Orders that exceeded estimated time
        public Dictionary<Station, int> OrdersByStation { get; set; }
    }

    /// <summary>
    /// WebSocket message for KDS real-time updates
    /// </summary>
    public class KDSWebSocketMessage
    {
        public string MessageType { get; set; } // order-received, order-updated, order-ready
        public KDSOrderResponse Order { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
