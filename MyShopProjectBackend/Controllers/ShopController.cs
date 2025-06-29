using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.ViewModels;
using System.Security.Claims;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopController : Controller
    {
        private readonly AppDbConection _context;

        public ShopController(AppDbConection conection)
        {
            _context = conection;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize (Roles ="Seller")]
        [HttpPost("CreateShop")]
        public async Task<IActionResult> CreateShop([FromBody] CreateShopModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdString = User.Identity?.Name;
            if (!int.TryParse(userIdString, out int userId))
            {
                // Додаткова перевірка клейм
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out userId))
                {
                    return Unauthorized("Invalid user identity");
                }
            }


            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (user.Role != "Seller")
            {
                return StatusCode(StatusCodes.Status403Forbidden, "Only sellers can create shops");
            }

            var shop = new Models.Shop
            {
                Name = model.Name,
                Description = model.Description,
                OwnerId = user.Id

            };
            await _context.shops.AddAsync(shop);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Магазин успішно створено", shopId = shop.Id });

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

            var shop = await _context.shops.FindAsync(model.ShopId);

            if(shop == null)
            {
                return NotFound("Магазин не знайдено");
            }

            if(shop.OwnerId != userId)
            {
                return Forbid("Ви не маєте прав змінювати цей магазин");
            }

            shop.Name = model.Name;
            shop.Description = model.Description;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Магазин успішно оновлено" });
        }


        [Authorize(Roles = "Seller")]
        [HttpPost("DeleteShop")]
        public async Task<IActionResult> DeleteShop(int shopId)
        {
            var userIdClime = User.Claims.FirstOrDefault(c=> c.Type == ClaimTypes.NameIdentifier);

            if (userIdClime == null || !int.TryParse(userIdClime.Value, out int userId)) 
            {
                return Unauthorized("Invalid user identity");
            }

            var shop = await _context.shops.FindAsync(shopId);
            if (shop == null) { 
            return NotFound();
            }

            if (shop.OwnerId != userId) {
                return Forbid("You do not have permission to delete this shop");
            }

           _context.shops.Remove(shop);
            await _context.SaveChangesAsync();

            return Ok(new {massage = "Магазин успішно видалено" });
        }

        [HttpGet("GetShopById")]
        public async Task<IActionResult> GetShopById(int shopId)
        {
           var shop = await _context.shops.FindAsync(shopId);

            if (shop == null)
            {
                return NotFound("Магазин не знайдено");
            }

            return Ok(shop);
        }
        [Authorize(Roles = "Seller")]
        [HttpGet("GetAllShops")]
        public async Task<IActionResult> GetAllShops(int ownerId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid user identity");
            }

            var shops = await _context.shops.Where(s => s.OwnerId == ownerId).ToListAsync();

            return Ok(shops);
        }


    }
}
