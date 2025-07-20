using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Models.Shop;
using MyShopProjectBackend.Servises.Interface;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/shop")]
    public class ShopController : ControllerBase
    {
        private readonly IShopService _shopService;

        public ShopController(IShopService shopService)
        {
            _shopService = shopService;
        }

        [HttpGet("status")]
        public IActionResult Get() => Ok("API працює");//готово

        [Authorize(Roles = "Seller")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopModel model)//готово
        {
            await _shopService.CreateShopAsync(model, User.GetUserId());

            return Ok(new { message = "Магазин успішно створений" });
        }

        [Authorize(Roles = "Seller")]
        [HttpPut]
        public async Task<IActionResult> UpdateShop([FromBody] UpdateShopModel model)//готово
        {
            await _shopService.UpdateShopAsync(model, User.GetUserId());
            return Ok(new { message = "Магазин успішно оновлено" });
        }


        [Authorize(Roles = "Seller")]
        [HttpDelete("{shopId}")]
        public async Task<IActionResult> DeleteShop(int shopId)//готово
        { 
            await _shopService.DeleteShopAsync(shopId, User.GetUserId()); 
            return Ok(new { message = "Магазин успішно видалено" });
        }

        [HttpGet("shop")]
        public async Task<IActionResult> GetShopById(int shopId)//готово
        {
            var result = await _shopService.GetShopByIdAsync(shopId);
            return Ok(result);
        }

        [Authorize(Roles = "Seller")]
        [HttpGet("shops")]//готово
        public async Task<IActionResult> GetAllShops()
        {
            var result = await _shopService.GetAllShopsAsync(User.GetUserId());
            return Ok(result);
        }
    }
}
