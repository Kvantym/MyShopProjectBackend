using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Models.Order;
using MyShopProjectBackend.Servises.Interface;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet("status")]
        public IActionResult Get() => Ok("API працює");//готово

        [Authorize(Roles = "Seller")]
        [HttpGet("by-user")]
        public async Task<IActionResult> GetOrdersForUser(string buyerName)//готово
        {
            var result = await _orderService.GetOrdersForUserAsync(buyerName, User.GetUserId());
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("by-Id")]
        public async Task<IActionResult> GetOrderById(int orderId)//готово
        {
            var result = await _orderService.GetOrderByIdAsync(orderId, User.GetUserId()); 
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPut]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderModel model)//готово
        {
            await _orderService.UpdateOrderStatusAsync(model, User.GetUserId());
            return Ok(new { message = "Статус замовлення оновлено", newStatus = model.Status });
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)//готово
        {
            await _orderService.DeleteOrderAsync(orderId, User.GetUserId());
            return Ok(new { message = "Замовлення видалено" });
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders(int shopId)
        {
          var result = await _orderService.GetAllOrdersAsync(shopId, User.GetUserId()); 
            return Ok(result);
        }
    }
}
