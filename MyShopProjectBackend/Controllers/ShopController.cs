using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Create;
using MyShopProjectBackend.ViewModels.Delete;
using MyShopProjectBackend.ViewModels.Update;
using System.Security.Claims;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly IShopServise _shopServise;

        public ShopController(IShopServise shopServise)
        {
            _shopServise = shopServise;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return Ok("Shop Controller is working");
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("CreateShop")]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopModel model)
        {
            model.OwnerId = User.GetUserId();
            var result = _shopServise.CreateShopAsync(model);
           
            return Ok(new { message = "Магазин успішно створений" });
        }

        [Authorize(Roles = "Seller")]
        [HttpPost("UpdateShop")]
        public async Task<IActionResult> UpdateShop([FromBody] UpdateShopModel model)
        {
            model.OwnerId = User.GetUserId(); 
            var result = _shopServise.UpdateShopAsync(model); 

            return Ok(new { message = "Магазин успішно оновлено" });
        }


        [Authorize(Roles = "Seller")]
        [HttpPost("DeleteShop")]
        public async Task<IActionResult> DeleteShop(DeleteShopModel model)
        {
            model.OwnerId = User.GetUserId(); 
            var result = _shopServise.DeleteShopAsync(model); 

            return Ok(new { message = "Магазин успішно видалено" });
        }

        [HttpGet("GetShopById")]
        public async Task<IActionResult> GetShopById(int shopId)
        {
            var result = await _shopServise.GetShopByIdAsync(shopId);

            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("GetAllShops")]
        public async Task<IActionResult> GetAllShops()
        {
            var result = await _shopServise.GetAllShopsAsync(User.GetUserId());

            return Ok(result);
        }
    }
}
