using ElegantCorner.Application.DTOs.Order;

namespace ElegantCorner.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetUserOrdersAsync(string userId);
        Task<OrderDto?> GetByIdAsync(Guid id, string userId);
        Task<OrderDto> CreateFromCartAsync(string userId);
        Task<bool> CancelOrderAsync(Guid id, string userId);
    }
}