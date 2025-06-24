using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbConection _context;

        public ShopController(AppDbConection conection)
        {
            _context = conection;
        }

        // GET: ShopController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopModel model)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateShop([FromBody] UpdateShopModel model)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteShop(int shopId)
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetShopById(int shopId)
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShops(int ownerId)
        {
            return Ok();
        }


    }
}
