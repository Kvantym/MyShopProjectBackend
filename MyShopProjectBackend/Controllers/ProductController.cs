using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;
using System.Security.Claims;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly AppDbConection _context;
        private readonly IProductServises _productServises;
        public ProductController(AppDbConection conection, IProductServises productServises)
        {
            _context = conection;
            _productServises = productServises;
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

            model.OwnerId = userId; // Прив'язка ID користувача до моделі

            var result = await _productServises.AddProductAsync(model); // Виклик сервісу для додавання товару
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо додавання не вдалось
            }

            return Ok(new { message = "Товар успішно додано" });
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
            model.OwnerId = userId; // Прив'язка ID користувача до моделі

            var result = await _productServises.UpdateProductAsync(model);// Виклик сервісу для оновлення товару
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);// Повернення помилки, якщо оновлення не вдалось
            }

            return Ok(new { message = "Товар оновлено успішно" });
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(DeleteProductModel model)
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
            model.OwnerId = userId; // Прив'язка ID користувача до моделі

            var result = await _productServises.DeleteProductAsync(model);// Виклик сервісу для видалення товару
            if (!result.Success) { 
            return BadRequest(result.ErrorMessage);// Повернення помилки, якщо видалення не вдалось
            }
            return Ok(new { message = "Товар успішно видалено" });
        }
        
        [HttpGet("GetProductByName")]
        public async Task<IActionResult> GetProductByName(string productName)
        {
            var result = await _productServises.GetProductByNameAsync(productName);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Products);
        }
        [HttpGet("GetProductsByShop")]
        public async Task<IActionResult> GetProductsByShop(int shopId)
        {
           var result = await _productServises.GetProductsByShopAsync(shopId);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Products);
        }
        //197
    }
}
