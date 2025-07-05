using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Отримання ідентифікатора користувача з токена
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found");
            }
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID");
            }
            model.OwnerId = userId; // Додаємо ідентифікатор власника до моделі

            var result = await _shopServise.CreateShopAsync(model); // Виклик методу для створення магазину
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо створення не вдалось
            }
            return Ok(new { message = "Магазин успішно створений" });
        }
        [Authorize(Roles = "Seller")]
        [HttpPost("UpdateShop")]
        public async Task<IActionResult> UpdateShop([FromBody] UpdateShopModel model)
        {
            var userIdClime = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClime == null || !int.TryParse(userIdClime.Value, out int userId))
            {
                return Unauthorized("Invalid user identity");
            }
            model.OwnerId = userId; // Додаємо ідентифікатор власника до моделі

            var result = await _shopServise.UpdateShopAsync(model); // Виклик методу для оновлення магазину
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо оновлення не вдалось
            }

            return Ok(new { message = "Магазин успішно оновлено" });
        }


        [Authorize(Roles = "Seller")]
        [HttpPost("DeleteShop")]
        public async Task<IActionResult> DeleteShop(DeleteShopModel model)
        {
            var userIdClime = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClime == null || !int.TryParse(userIdClime.Value, out int userId))
            {
                return Unauthorized("Invalid user identity");
            }
            model.OwnerId = userId; // Додаємо ідентифікатор власника до моделі

            var result = await _shopServise.DeleteShopAsync(model); // Виклик методу для видалення магазину
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо видалення не вдалось
            }

            return Ok(new { message = "Магазин успішно видалено" });

        }

        [HttpGet("GetShopById")]
        public async Task<IActionResult> GetShopById(int shopId)
        {
            var result = await _shopServise.GetShopByIdAsync(shopId); // Виклик методу для отримання магазину за ідентифікатором
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо отримання не вдалось
            }
            return Ok(result.Shop);
        }
        [Authorize(Roles = "Seller")]
        [HttpGet("GetAllShops")]
        public async Task<IActionResult> GetAllShops()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid user identity");
            }

            var result = await _shopServise.GetAllShopsAsync(userId); // Виклик методу для отримання всіх магазинів
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо отримання не вдалось
            }

            return Ok(result.Shops);
        }


    }
}
