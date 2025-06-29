using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.ViewModels;
using System.Security.Claims;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly AppDbConection _context;

        public ProductController(AppDbConection conection)
        {
            _context = conection;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Seller")]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdClime = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClime == null || !int.TryParse(userIdClime.Value, out int userId))
            {
                return Unauthorized("Invalid user identity");
            }

          var shop = await _context.shops.FindAsync(model.ShopId);

            if (shop == null) { 
            return NotFound("Магазин не знайдено");
            }

            if (shop.OwnerId != userId) {
                return Forbid("Ви не маєте права додавати товар до чужого магазину");
            }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Category = model.Category,
                Price = model.Price,
                ShopId = shop.Id,
                Quantity = model.Quantity,
                ImageData = model.ImageData,
                ImageMimeType = model.ImageMimeType,
            };

            await _context.products.AddAsync(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Товар успішно додано", productId = product.Id });
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdClime = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);// Отримання ID користувача з клеймів 

            if (userIdClime == null || !int.TryParse(userIdClime.Value, out int userId))// Перевірка, чи є користувач авторизованим
            {
                return Unauthorized("Invalid user identity");
            }
            var product = await _context.products.FindAsync(model.ProductId);// Отримання товару за його ID
            if (product == null) { 
            return NotFound("Товар не знайдено");
            }
            var shop = await _context.shops.FindAsync(product.ShopId);// Отримання магазину, до якого належить товар
            if (shop == null) { 
            return NotFound("Магазин товару не знайдено");
            }

            if(shop.OwnerId != userId)
            {
                return Forbid("Ви не маєте права редагувати товар з чужого магазину");
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.Category = model.Category;
            product.Price = model.Price;
            product.Quantity = model.Quantity;
            product.ImageData = model.ImageData;
            product.ImageMimeType = model.ImageMimeType;

            await _context.SaveChangesAsync();// Збереження змін у базі даних

            return Ok(new { message = "Товар оновлено успішно", productId = product.Id });
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            var userIdClime = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);// Отримання ID користувача з клеймів 

            if (userIdClime == null || !int.TryParse(userIdClime.Value, out int userId))// Перевірка, чи є користувач авторизованим
            {
                return Unauthorized("Invalid user identity");
            }

            var product = await _context.products.FindAsync(productId);// Отримання товару за його ID

            if (product == null) // Перевірка, чи товар існує
            { 
            return NotFound("Товар не знайдено");
            }

            var shop = await _context.shops.FindAsync(product.ShopId);// Отримання магазину, до якого належить товар
            if (shop == null) 
            {
                return NotFound("Магазин товару не знайдено");
            }

            if(shop.OwnerId != userId)// Перевірка, чи є користувач власником магазину
            {
             return Forbid("Ви не маєте права видаляти товар з чужого магазину");
            }
            _context.products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Товар успішно видалено", productId = productId });
        }
        [HttpGet("GetProductById")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var product = await _context.products.FindAsync(productId);

            if (product == null)
            {
                return NotFound("Товар не знайдено");
            }

            return Ok(product);
        }
        [HttpGet("GetProductsByShop")]
        public async Task<IActionResult> GetProductsByShop(int shopId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shop = await _context.shops.FindAsync(shopId);

            if(shop == null)
            {
                return NotFound("Магазин не знайдено");
            }

            var products = await _context.products.Where(p => p.ShopId == shopId).Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Category = p.Category,
                Price = p.Price,
                ShopId = p.ShopId,
                Quantity = p.Quantity,
                ImageData = p.ImageData,
                ImageMimeType = p.ImageMimeType
            }).ToListAsync();

            return Ok(products);
        }

    }
}
