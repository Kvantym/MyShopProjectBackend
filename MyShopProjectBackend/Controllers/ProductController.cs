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
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            ;
            _productService = productService;
        }
        [HttpGet("status")]
        public IActionResult Get() => Ok("API працює");//готово
        [Authorize(Roles = "Seller")]
        [HttpPost()]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductModel model)//готово
        {
            await _productService.AddProductAsync(model, User.GetUserId());
            return Ok(new { message = "Товар успішно додано" });
        }

        [Authorize(Roles = "Seller")]
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductModel model)//готово
        {
            await _productService.UpdateProductAsync(model, User.GetUserId());
            return Ok(new { message = "Товар оновлено успішно" });
        }

        [Authorize(Roles = "Seller")]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)//готово
        {
            await _productService.DeleteProductAsync(productId, User.GetUserId());
            return Ok(new { message = "Товар успішно видалено" });
        }

        [HttpGet("by-name/{productName}")]
        public async Task<IActionResult> GetProductByName(string productName)//готово
        {
            var result = await _productService.GetProductsByNameAsync(productName);
            return Ok(result);
        }
        [HttpGet("by-shop/{shopId}")]
        public async Task<IActionResult> GetProductsByShop(int shopId)//готово
        {
            var result = await _productService.GetProductsByShopAsync(shopId);
            return Ok(result);
        }
        [HttpGet("products")]
        public async Task<IActionResult> GetProducts()
        {
            var result = await _productService.GetProductsAsync();
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            var products = await _productService.SearchProductsByNameAsync(name);
            return Ok(products);
        }
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)//готово
        {
            var result = await _productService.GetProductsByIdAsync(productId);
            return Ok(result);
        }
    }
}
