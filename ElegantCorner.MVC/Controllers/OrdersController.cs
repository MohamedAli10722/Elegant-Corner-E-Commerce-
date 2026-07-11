using ElegantCorner.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElegantCorner.MVC.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // AJAX: POST /Orders/CreateFromCart
        [HttpPost]
        public async Task<IActionResult> CreateFromCart()
        {
            try
            {
                var order = await _orderService.CreateFromCartAsync(UserId);
                return Json(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // AJAX: GET /Orders/MyOrders
        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var orders = await _orderService.GetUserOrdersAsync(UserId);
            return Json(orders);
        }

        // AJAX: POST /Orders/Cancel
        [HttpPost]
        public async Task<IActionResult> Cancel([FromBody] CancelRequest req)
        {
            var result = await _orderService.CancelOrderAsync(req.OrderId, UserId);
            if (!result) return BadRequest(new { message = "Cannot cancel this order" });
            return Ok();
        }
    }

    public record CancelRequest(Guid OrderId);
}
