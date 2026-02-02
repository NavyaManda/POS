using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Services
{
    /// <summary>
    /// Order service for processing orders
    /// </summary>
    public interface IOrderService
    {
        Task<Models.CreateOrderResponse> CreateOrderAsync(Models.CreateOrderRequest request);
        Task<Models.OrderDetailsResponse> GetOrderAsync(string orderId);
        Task<List<Models.OrderDetailsResponse>> GetOrdersByCustomerAsync(string customerId);
        Task UpdateOrderStatusAsync(string orderId, Models.OrderStatus status);
        Task CancelOrderAsync(string orderId);
        Task UpdateOrderAsync(string orderId, Models.UpdateOrderRequest request);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IMenuServiceClient _menuServiceClient;
        private readonly IInventoryServiceClient _inventoryServiceClient;

        public OrderService(
            IOrderRepository orderRepository,
            IEventPublisher eventPublisher,
            IMenuServiceClient menuServiceClient,
            IInventoryServiceClient inventoryServiceClient)
        {
            _orderRepository = orderRepository;
            _eventPublisher = eventPublisher;
            _menuServiceClient = menuServiceClient;
            _inventoryServiceClient = inventoryServiceClient;
        }

        public async Task<Models.CreateOrderResponse> CreateOrderAsync(Models.CreateOrderRequest request)
        {
            // Validate menu items and get prices
            var menuItems = await _menuServiceClient.GetMenuItemsAsync(
                request.Items.Select(x => x.MenuItemId).ToList());

            if (menuItems.Count != request.Items.Count)
            {
                throw new InvalidOperationException("One or more menu items not found");
            }

            // Create order items with prices from menu service
            var orderItems = request.Items.Select((item, idx) =>
            {
                var menuItem = menuItems[idx];
                return new Models.OrderItem
                {
                    MenuItemId = item.MenuItemId,
                    MenuItemName = menuItem.Name,
                    Quantity = item.Quantity,
                    UnitPrice = menuItem.Price,
                    SpecialInstructions = item.SpecialInstructions
                };
            }).ToList();

            // Reserve inventory
            var reservationSuccess = await _inventoryServiceClient.ReserveItemsAsync(
                orderItems.Select(x => new { x.MenuItemId, x.Quantity }).ToList());

            if (!reservationSuccess)
            {
                throw new InvalidOperationException("Unable to reserve inventory");
            }

            // Create order in database
            var order = new Models.Order
            {
                CustomerId = request.CustomerId,
                Items = orderItems,
                TotalAmount = orderItems.Sum(x => x.Subtotal),
                Status = Models.OrderStatus.PENDING,
                Notes = request.Notes,
                OrderNumber = GenerateOrderNumber()
            };

            await _orderRepository.CreateAsync(order);

            // Publish OrderCreated event
            await _eventPublisher.PublishAsync(new POS.Shared.Models.EventMessage
            {
                EventType = "order.created.v1",
                Source = "OrderService",
                Payload = new Dictionary<string, object>
                {
                    { "OrderId", order.OrderId },
                    { "OrderNumber", order.OrderNumber },
                    { "CustomerId", order.CustomerId },
                    { "TotalAmount", order.TotalAmount },
                    { "Items", order.Items }
                }
            });

            return new Models.CreateOrderResponse
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(x => new Models.OrderItemResponse
                {
                    MenuItemId = x.MenuItemId,
                    MenuItemName = x.MenuItemName,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Subtotal = x.Subtotal
                }).ToList()
            };
        }

        public async Task<Models.OrderDetailsResponse> GetOrderAsync(string orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            return MapToResponse(order);
        }

        public async Task<List<Models.OrderDetailsResponse>> GetOrdersByCustomerAsync(string customerId)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(customerId);
            return orders.Select(MapToResponse).ToList();
        }

        public async Task UpdateOrderStatusAsync(string orderId, Models.OrderStatus status)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            order.Status = status;

            if (status == Models.OrderStatus.CONFIRMED)
            {
                order.ConfirmedAt = DateTime.UtcNow;
            }
            else if (status == Models.OrderStatus.READY)
            {
                order.ReadyAt = DateTime.UtcNow;
            }
            else if (status == Models.OrderStatus.COMPLETED)
            {
                order.CompletedAt = DateTime.UtcNow;
            }

            await _orderRepository.UpdateAsync(order);

            // Publish status change event
            var eventType = status switch
            {
                Models.OrderStatus.CONFIRMED => "order.confirmed.v1",
                Models.OrderStatus.PREPARING => "order.preparing.v1",
                Models.OrderStatus.READY => "order.ready.v1",
                Models.OrderStatus.COMPLETED => "order.completed.v1",
                Models.OrderStatus.CANCELLED => "order.cancelled.v1",
                _ => null
            };

            if (eventType != null)
            {
                await _eventPublisher.PublishAsync(new POS.Shared.Models.EventMessage
                {
                    EventType = eventType,
                    Source = "OrderService",
                    Payload = new Dictionary<string, object>
                    {
                        { "OrderId", orderId },
                        { "Status", status.ToString() }
                    }
                });
            }
        }

        public async Task CancelOrderAsync(string orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            if (order.Status == Models.OrderStatus.COMPLETED)
            {
                throw new InvalidOperationException("Cannot cancel completed order");
            }

            // Release inventory
            await _inventoryServiceClient.ReleaseItemsAsync(
                order.Items.Select(x => new { x.MenuItemId, x.Quantity }).ToList());

            order.Status = Models.OrderStatus.CANCELLED;
            await _orderRepository.UpdateAsync(order);

            // Publish OrderCancelled event
            await _eventPublisher.PublishAsync(new POS.Shared.Models.EventMessage
            {
                EventType = "order.cancelled.v1",
                Source = "OrderService",
                Payload = new Dictionary<string, object> { { "OrderId", orderId } }
            });
        }

        public async Task UpdateOrderAsync(string orderId, Models.UpdateOrderRequest request)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            if (order.Status != Models.OrderStatus.PENDING)
            {
                throw new InvalidOperationException("Can only modify pending orders");
            }

            order.Items = request.Items.Select(x => new Models.OrderItem
            {
                MenuItemId = x.MenuItemId,
                Quantity = x.Quantity,
                SpecialInstructions = x.SpecialInstructions
            }).ToList();

            order.Notes = request.Notes;
            await _orderRepository.UpdateAsync(order);

            // Publish OrderModified event
            await _eventPublisher.PublishAsync(new POS.Shared.Models.EventMessage
            {
                EventType = "order.modified.v1",
                Source = "OrderService",
                Payload = new Dictionary<string, object> { { "OrderId", orderId } }
            });
        }

        private Models.OrderDetailsResponse MapToResponse(Models.Order order)
        {
            return new Models.OrderDetailsResponse
            {
                OrderId = order.OrderId,
                OrderNumber = order.OrderNumber,
                CustomerId = order.CustomerId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                ConfirmedAt = order.ConfirmedAt,
                ReadyAt = order.ReadyAt,
                Items = order.Items.Select(x => new Models.OrderItemResponse
                {
                    MenuItemId = x.MenuItemId,
                    MenuItemName = x.MenuItemName,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Subtotal = x.Subtotal
                }).ToList()
            };
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow.Ticks % 1000000}";
        }
    }

    /// <summary>
    /// Repository interface for order persistence
    /// </summary>
    public interface IOrderRepository
    {
        Task<Models.Order> GetByIdAsync(string orderId);
        Task<List<Models.Order>> GetByCustomerIdAsync(string customerId);
        Task CreateAsync(Models.Order order);
        Task UpdateAsync(Models.Order order);
    }

    /// <summary>
    /// Event publisher interface
    /// </summary>
    public interface IEventPublisher
    {
        Task PublishAsync(POS.Shared.Models.EventMessage eventMessage);
    }

    /// <summary>
    /// Menu Service HTTP client
    /// </summary>
    public interface IMenuServiceClient
    {
        Task<List<MenuItemDto>> GetMenuItemsAsync(List<string> menuItemIds);
    }

    public class MenuItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
    }

    /// <summary>
    /// Inventory Service HTTP client
    /// </summary>
    public interface IInventoryServiceClient
    {
        Task<bool> ReserveItemsAsync(List<dynamic> items);
        Task<bool> ReleaseItemsAsync(List<dynamic> items);
    }
}
