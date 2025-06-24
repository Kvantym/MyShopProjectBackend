using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbConection _context;

        public ProductController(AppDbConection conection)
        {
            _context = conection;
        }

        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductModel model)
        {
            // Logic to create a product
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductModel model)
        {
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetProductById(int productId)
        {
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetProductsByShop(int shopId)
        {
            return Ok();
        }


    }
}
