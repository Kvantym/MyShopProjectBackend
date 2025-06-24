using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Db;

namespace MyShopProjectBackend.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbConection _context;

        public OrderController(AppDbConection conection)
        {
            _context = conection;
        }

        // GET: OrderController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(int userId, List<int> productIds)
        {
           
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersByUser(int userId) 
        {
            
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderByShop(int shopId)
        {
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            return Ok();
        }

    }
}
