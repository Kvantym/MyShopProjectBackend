using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Create;
using MyShopProjectBackend.ViewModels.Delete;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductServises _productServises;
        public ProductController(IProductServises productServises)
        {;
            _productServises = productServises;
        }
        [HttpGet]
        public ActionResult Index()
        {
            return Ok("Product Controller is working");
        }
        [Authorize(Roles = "Seller")]
        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductModel model)
        {
            model.OwnerId = User.GetUserId();
            var result =  _productServises.AddProductAsync(model);

            return Ok(new { message = "Товар успішно додано" });
        }

        [Authorize(Roles = "Seller")]
        [HttpPost]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductModel model)
        {
            model.OwnerId = User.GetUserId(); 
            var result = _productServises.UpdateProductAsync(model);

            return Ok(new { message = "Товар оновлено успішно" });
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(DeleteProductModel model)
        {
            model.OwnerId = User.GetUserId();
            var result = _productServises.DeleteProductAsync(model);

            return Ok(new { message = "Товар успішно видалено" });
        }
        
        [HttpGet("GetProductByName")]
        public async Task<IActionResult> GetProductByName(string productName)
        {
            var result = await _productServises.GetProductByNameAsync(productName);
            return Ok(result);
        }
        [HttpGet("GetProductsByShop")]
        public async Task<IActionResult> GetProductsByShop(int shopId)
        {
           var result = await _productServises.GetProductsByShopAsync(shopId);
            return Ok(result);
        }
    }
}
