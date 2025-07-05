using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;

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

        // GET: OrderController
        [HttpGet]
        public ActionResult Index()
        {
          return Ok("Order Controller is working");
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("GetOrdersForUser")]
        public async Task<IActionResult> GetOrdersForUser(int buyerId)
        {
            var sellerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (sellerIdClaim == null || !int.TryParse(sellerIdClaim, out int sellerId))
            {
                return Unauthorized("Некоректний ідентифікатор продавця");
            }

            var result = await _orderServises.GetOrdersForUserAsync(buyerId,sellerId);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Orders);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("GetOrderById")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var sellerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (sellerIdClaim == null || !int.TryParse(sellerIdClaim, out int sellerId))
            {
                return Unauthorized("Некоректний ідентифікатор продавця");
            }

            var result = await _orderServises.GetOrderByIdAsync(orderId, sellerId);

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Order);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus(UpdateOrderModel model)
        {

            var sellerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (sellerIdClaim == null || !int.TryParse(sellerIdClaim, out int sellerId))
            {
                return Unauthorized("Некоректний ідентифікатор продавця");
            } 
           
            model.SellerId = sellerId;
            var result = await _orderServises.UpdateOrderStatusAsync(model);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(new { message = "Статус замовлення оновлено", newStatus = model.Status });
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var sellerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (sellerIdClaim == null || !int.TryParse(sellerIdClaim, out int sellerId))
            {
                return Unauthorized("Некоректний ідентифікатор продавця");
            }

            var result = await _orderServises.DeleteOrderAsync(orderId, sellerId);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(new { message = "Замовлення видалено" });
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders(int shopId)
        {
            var sellerIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (sellerIdClaim == null || !int.TryParse(sellerIdClaim, out int sellerId))
            {
                return Unauthorized("Некоректний ідентифікатор продавця");
            }

          var result = await _orderServises.GetAllOrdersAsync(shopId, sellerId);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            
            return Ok(result.Orders);
        }

       
    }
}
