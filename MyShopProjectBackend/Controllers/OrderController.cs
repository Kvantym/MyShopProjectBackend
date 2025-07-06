using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderServises _orderServises;

        public OrderController(IOrderServises orderServises)
        {
            _orderServises = orderServises;
        }
        [HttpGet]
        public ActionResult Index()
        {
          return Ok("Order Controller is working");
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("GetOrdersForUser")]
        public async Task<IActionResult> GetOrdersForUser(string buyerName)
        {
            var sellerId = User.GetUserId();
            var result = await _orderServises.GetOrdersForUserAsync(buyerName, sellerId);

            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("GetOrderById")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var result = await _orderServises.GetOrderByIdAsync(orderId, User.GetUserId()); //User.GetUserId() ID продавця);
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderModel model)
        {
            model.SellerId = User.GetUserId();
            var result = _orderServises.UpdateOrderStatusAsync(model);

            return Ok(new { message = "Статус замовлення оновлено", newStatus = model.Status });
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var result = _orderServises.DeleteOrderAsync(orderId, User.GetUserId()); //User.GetUserId() ID продавця
            return Ok(new { message = "Замовлення видалено" });
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders(int shopId)
        {
          var result = await _orderServises.GetAllOrdersAsync(shopId, User.GetUserId()); //User.GetUserId() ID продавця
            return Ok(result);
        }
    }
}//125
