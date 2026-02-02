using System;
using System.Collections.Generic;

namespace OrderService.Models
{
    /// <summary>
    /// Order status enumeration
    /// </summary>
    public enum OrderStatus
    {
        PENDING,
        CONFIRMED,
        PREPARING,
        READY,
        PICKED_UP,
        COMPLETED,
        CANCELLED,
        PAYMENT_FAILED
    }

    /// <summary>
    /// Order entity
    /// </summary>
    public class Order
    {
        public string OrderId { get; set; } = Guid.NewGuid().ToString();
        public string OrderNumber { get; set; } // Human-readable: ORD-001
        public string CustomerId { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.PENDING;
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ReadyAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string PaymentId { get; set; }
    }

    /// <summary>
    /// Order item (line item in order)
    /// </summary>
    public class OrderItem
    {
        public string ItemId { get; set; } = Guid.NewGuid().ToString();
        public string MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal => Quantity * UnitPrice;
        public List<string> SpecialInstructions { get; set; } = new();
    }

    /// <summary>
    /// Create order request
    /// </summary>
    public class CreateOrderRequest
    {
        public string CustomerId { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
        public string Notes { get; set; }
    }

    /// <summary>
    /// Order item request DTO
    /// </summary>
    public class OrderItemRequest
    {
        public string MenuItemId { get; set; }
        public int Quantity { get; set; }
        public List<string> SpecialInstructions { get; set; } = new();
    }

    /// <summary>
    /// Create order response
    /// </summary>
    public class CreateOrderResponse
    {
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemResponse> Items { get; set; }
    }

    /// <summary>
    /// Order item response DTO
    /// </summary>
    public class OrderItemResponse
    {
        public string MenuItemId { get; set; }
        public string MenuItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// Order details response
    /// </summary>
    public class OrderDetailsResponse
    {
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderItemResponse> Items { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ReadyAt { get; set; }
    }

    /// <summary>
    /// Update order request
    /// </summary>
    public class UpdateOrderRequest
    {
        public List<OrderItemRequest> Items { get; set; }
        public string Notes { get; set; }
    }
}
