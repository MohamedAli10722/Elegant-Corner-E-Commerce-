using ElegantCorner.Application.DTOs.Cart;
using ElegantCorner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElegantCorner.MVC.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // AJAX: GET /Cart
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cart = await _cartService.GetCartAsync(UserId);
            return Json(cart ?? new CartDto());
        }

        // AJAX: POST /Cart/AddItem
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] AddToCartDto dto)
        {
            var cart = await _cartService.AddItemAsync(UserId, dto);
            return Json(cart);
        }

        // AJAX: POST /Cart/UpdateItem
        [HttpPost]
        public async Task<IActionResult> UpdateItem([FromBody] UpdateItemRequest req)
        {
            await _cartService.UpdateItemAsync(UserId, req.CartItemId, req.Quantity);
            var cart = await _cartService.GetCartAsync(UserId);
            return Json(cart ?? new CartDto());
        }

        // AJAX: POST /Cart/RemoveItem
        [HttpPost]
        public async Task<IActionResult> RemoveItem([FromBody] RemoveItemRequest req)
        {
            await _cartService.RemoveItemAsync(UserId, req.CartItemId);
            var cart = await _cartService.GetCartAsync(UserId);
            return Json(cart ?? new CartDto());
        }

        // AJAX: POST /Cart/Clear
        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            await _cartService.ClearCartAsync(UserId);
            return Ok();
        }
    }

    public record UpdateItemRequest(Guid CartItemId, int Quantity);
    public record RemoveItemRequest(Guid CartItemId);
}
