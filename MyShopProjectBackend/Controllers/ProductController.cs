using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Models.Product;
using MyShopProjectBackend.Servises.Interface;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/product")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productServises;
        public ProductController(IProductService productServises)
        {
            ;
            _productServises = productServises;
        }
        [HttpGet("status")]
        public IActionResult Get() => Ok("API працює");//готово
        [Authorize(Roles = "Seller")]
        [HttpPost()]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductModel model)//готово
        {
            await _productServises.AddProductAsync(model, User.GetUserId());
            return Ok(new { message = "Товар успішно додано" });
        }

        [Authorize(Roles = "Seller")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductModel model)//готово
        {
            await _productServises.UpdateProductAsync(model, User.GetUserId());
            return Ok(new { message = "Товар оновлено успішно" });
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)//готово
        {
            await _productServises.DeleteProductAsync(productId, User.GetUserId());
            return Ok(new { message = "Товар успішно видалено" });
        }

        [HttpGet("by-name/{productName}")]
        public async Task<IActionResult> GetProductByName(string productName)//готово
        {
            var result = await _productServises.GetProductsByNameAsync(productName);
            return Ok(result);
        }
        [HttpGet("by-shop/{shopId}")]
        public async Task<IActionResult> GetProductsByShop(int shopId)//готово
        {
            var result = await _productServises.GetProductsByShopAsync(shopId);
            return Ok(result);
        }
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var result = await _productServises.GetProductsAsync();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var products = await _productServises.SearchProductsByNameAsync(name);
            return Ok(products);
        }
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)//готово
        {
            var result = await _productServises.GetProductsByIdAsync(productId);
            return Ok(result);
        }
    }
}
