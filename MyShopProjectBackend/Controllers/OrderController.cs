using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly AppDbConection _context;

        public OrderController(AppDbConection conection)
        {
            _context = conection;
        }

        // GET: OrderController
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("GetOrdersForUser")]
        public async Task<IActionResult> GetOrdersForUser(int userId)
        {
            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Користувача не знайдено");
            }

            if (!User.IsInRole("Seller"))
            {
                return BadRequest("Можна тільки продавцю");
            }

            var orders = await _context.orders
                .Where(o => o.BuyerId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound("Замовлення не знайдено для цього користувача");
            }

            var orderDtos = GetOrderDtos(orders);

            return Ok(orderDtos);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("GetOrderById")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _context.orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Замовлення не знайдено");
            }

            if (!User.IsInRole("Seller"))
            {
                return BadRequest("Можна тільки продавцю");
            }

            var orderDto = new OrderDto
            {
                OrderId = order.Id,
                Status = order.Status,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    ProductName = oi.Product.Name
                }).ToList()
            };

            return Ok(orderDto);
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] string status)
        {
            if (!User.IsInRole("Seller"))
            {
                return BadRequest("Можна тільки продавцю");
            }

            var order = await _context.orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Замовлення не знайдено");
            }

            var validStatuses = new[]
            {
                ShopOrderStatus.Pending,
                ShopOrderStatus.Completed,
                ShopOrderStatus.Cancelled,
                ShopOrderStatus.InProgress,
                ShopOrderStatus.Refunded,
                ShopOrderStatus.Shipped,
                ShopOrderStatus.Delivered,
                ShopOrderStatus.Confirmed
            };

            if (!validStatuses.Contains(status))
            {
                return BadRequest("Невірний статус");
            }

            if (order.Status == ShopOrderStatus.Completed.ToString() ||
                order.Status == ShopOrderStatus.Cancelled.ToString())
            {
                return BadRequest("Замовлення вже завершено або скасовано");
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Статус замовлення оновлено", newStatus = order.Status });
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            if (!User.IsInRole("Seller"))
            {
                return BadRequest("Можна тільки продавцю");
            }

            var order = await _context.orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound("Замовлення не знайдено");
            }

            var orderItems = await _context.orderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
            _context.orderItems.RemoveRange(orderItems);
            _context.orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Замовлення успішно видалено" });
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            if (!User.IsInRole("Seller"))
            {
                return BadRequest("Можна тільки продавцю");
            }

            var orders = await _context.orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound("Замовлення не знайдено");
            }

            var orderDtos = GetOrderDtos(orders);

            return Ok(orderDtos);
        }

        private List<OrderDto> GetOrderDtos(List<Order> orders)
        {
            return orders.Select(o => new OrderDto
            {
                OrderId = o.Id,
                Status = o.Status,
                Items = o.OrderItems.Select(oi => new OrderItemDto
                {
                    ProductId = oi.ProductId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    ProductName = oi.Product.Name
                }).ToList()
            }).ToList();
        }
    }
}
