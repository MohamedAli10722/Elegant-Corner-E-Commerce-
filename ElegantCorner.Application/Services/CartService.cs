using ElegantCorner.Application.DTOs.Cart;
using ElegantCorner.Application.Interfaces;
using ElegantCorner.Domain.Entities;
using ElegantCorner.Domain.Interfaces;

namespace ElegantCorner.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _uow;

        public CartService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        private static CartDto MapToDto(Cart cart) => new CartDto
        {
            Id = cart.Id,
            Items = cart.Items.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                ProductImage = i.Product?.ImageUrl,
                UnitPrice = i.Product?.Price ?? 0,
                Quantity = i.Quantity
            }).ToList()
        };

        private async Task LoadCartProducts(Cart cart)
        {
            foreach (var item in cart.Items)
            {
                var product = await _uow.Products.GetByIdAsync(item.ProductId);
                item.Product = product!;
            }
        }

        public async Task<CartDto?> GetCartAsync(string userId)
        {
            var carts = await _uow.Carts.GetAllWithIncludeAsync(c => c.Items);
            var cart = carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null) return null;
            await LoadCartProducts(cart);
            return MapToDto(cart);
        }

        public async Task<CartDto> AddItemAsync(string userId, AddToCartDto dto)
        {
            var carts = await _uow.Carts.GetAllWithIncludeAsync(c => c.Items);
            var cart = carts.FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _uow.Carts.AddAsync(cart);
                await _uow.SaveChangesAsync();
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
                _uow.CartItems.Update(existingItem);
            }
            else
            {
                var item = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity
                };
                await _uow.CartItems.AddAsync(item);
            }

            await _uow.SaveChangesAsync();

            carts = await _uow.Carts.GetAllWithIncludeAsync(c => c.Items);
            cart = carts.First(c => c.UserId == userId);
            await LoadCartProducts(cart);
            return MapToDto(cart);
        }

        public async Task<bool> UpdateItemAsync(string userId, Guid cartItemId, int quantity)
        {
            var item = await _uow.CartItems.GetByIdAsync(cartItemId);
            if (item == null) return false;
            if (quantity <= 0)
                _uow.CartItems.Delete(item);
            else
            {
                item.Quantity = quantity;
                _uow.CartItems.Update(item);
            }
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveItemAsync(string userId, Guid cartItemId)
        {
            var item = await _uow.CartItems.GetByIdAsync(cartItemId);
            if (item == null) return false;
            _uow.CartItems.Delete(item);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            var carts = await _uow.Carts.GetAllWithIncludeAsync(c => c.Items);
            var cart = carts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null) return false;
            foreach (var item in cart.Items.ToList())
                _uow.CartItems.Delete(item);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}