using ElegantCorner.Application.DTOs.Cart;
using ElegantCorner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElegantCorner.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _service;

        public CartController(ICartService service)
        {
            _service = service;
        }

        private string GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var result = await _service.GetCartAsync(GetUserId());
            if (result == null) return Ok(new { items = new List<object>(), total = 0 });
            return Ok(result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddItem(AddToCartDto dto)
        {
            var result = await _service.AddItemAsync(GetUserId(), dto);
            return Ok(result);
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<IActionResult> UpdateItem(Guid cartItemId, [FromBody] int quantity)
        {
            var result = await _service.UpdateItemAsync(GetUserId(), cartItemId, quantity);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> RemoveItem(Guid cartItemId)
        {
            var result = await _service.RemoveItemAsync(GetUserId(), cartItemId);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var result = await _service.ClearCartAsync(GetUserId());
            if (!result) return NotFound();
            return NoContent();
        }
    }
}