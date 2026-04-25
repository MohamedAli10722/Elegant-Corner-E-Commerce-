using ElegantCorner.Application.DTOs.Cart;

namespace ElegantCorner.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartDto?> GetCartAsync(string userId);
        Task<CartDto> AddItemAsync(string userId, AddToCartDto dto);
        Task<bool> UpdateItemAsync(string userId, Guid cartItemId, int quantity);
        Task<bool> RemoveItemAsync(string userId, Guid cartItemId);
        Task<bool> ClearCartAsync(string userId);
    }
}