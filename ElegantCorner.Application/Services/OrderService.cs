using ElegantCorner.Application.DTOs.Order;
using ElegantCorner.Application.Interfaces;
using ElegantCorner.Domain.Entities;
using ElegantCorner.Domain.Interfaces;

namespace ElegantCorner.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _uow;

        public OrderService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        private static OrderDto MapToDto(Order o) => new OrderDto
        {
            Id = o.Id,
            CreatedAt = o.CreatedAt,
            Status = o.Status,
            TotalPrice = o.TotalPrice,
            Items = o.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        public async Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId)
        {
            var orders = await _uow.Orders.GetAllAsync();
            return orders.Where(o => o.UserId == userId).Select(MapToDto);
        }

        public async Task<OrderDto?> GetByIdAsync(Guid id, string userId)
        {
            var order = await _uow.Orders.GetByIdAsync(id);
            if (order == null || order.UserId != userId) return null;
            return MapToDto(order);
        }

        public async Task<OrderDto> CreateFromCartAsync(string userId)
        {
            var carts = await _uow.Carts.GetAllWithIncludeAsync(c => c.Items);
            var cart = carts.FirstOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
                throw new Exception("Cart is empty");

            // Load products
            foreach (var item in cart.Items)
            {
                var product = await _uow.Products.GetByIdAsync(item.ProductId);
                item.Product = product!;
            }

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending,
                Items = cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.Product?.Price ?? 0
                }).ToList()
            };

            order.TotalPrice = order.Items.Sum(i => i.UnitPrice * i.Quantity);

            await _uow.Orders.AddAsync(order);

            foreach (var item in cart.Items.ToList())
                _uow.CartItems.Delete(item);

            await _uow.SaveChangesAsync();
            return MapToDto(order);
        }

        public async Task<bool> CancelOrderAsync(Guid id, string userId)
        {
            var order = await _uow.Orders.GetByIdAsync(id);
            if (order == null || order.UserId != userId) return false;
            if (order.Status != OrderStatus.Pending) return false;

            order.Status = OrderStatus.Cancelled;
            _uow.Orders.Update(order);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}