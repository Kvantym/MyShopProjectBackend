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
        private readonly IOrderServises _orderServises;

        public OrderController(IOrderServises orderServises)
        {
            _orderServises = orderServises;
        }
        [HttpGet("status")]
        public IActionResult Get() => Ok("API працює");//готово

        [Authorize(Roles = "Seller")]
        [HttpGet("by-user")]
        public async Task<IActionResult> GetOrdersForUser(string buyerName)//готово
        {
            var result = await _orderServises.GetOrdersForUserAsync(buyerName, User.GetUserId());
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("by-Id")]
        public async Task<IActionResult> GetOrderById(int orderId)//готово
        {
            var result = await _orderServises.GetOrderByIdAsync(orderId, User.GetUserId()); 
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderModel model)//готово
        {
            await _orderServises.UpdateOrderStatusAsync(model, User.GetUserId());
            return Ok(new { message = "Статус замовлення оновлено", newStatus = model.Status });
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)//готово
        {
            await _orderServises.DeleteOrderAsync(orderId, User.GetUserId());
            return Ok(new { message = "Замовлення видалено" });
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders(int shopId)
        {
          var result = await _orderServises.GetAllOrdersAsync(shopId, User.GetUserId()); 
            return Ok(result);
        }
    }
}
