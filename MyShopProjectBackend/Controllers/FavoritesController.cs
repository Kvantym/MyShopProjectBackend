using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.Models;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : Controller
    {
        private readonly AppDbConection _context;

        public FavoritesController(AppDbConection conection)
        {
            _context = conection;
        }

        // GET: FavoritesController
        [HttpGet]
        public ActionResult Index()
        {
            return Ok();
        }
        [Authorize]
        [HttpPost("AddToFavorites")]
        public async Task<IActionResult> AddToFavorites(int productId)
        {
            var userIdClime = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userIdClime == null) {
                return Unauthorized("Користувач не авторизований");
            }
            int userId = int.Parse(userIdClime);

            var product = await _context.products.FindAsync(productId);

            if(product == null)
            {
                return BadRequest("Товару не існує");
            }

            bool alreadyExists = await _context.favoritProducts.AnyAsync(fp => fp.UserId == userId && fp.ProductId == productId);

            if (alreadyExists)
            {
                return BadRequest("Товар вже додано до обраного");
            }

            var favoritProduct = new FavoritProduct { 
            UserId = userId,
            ProductId = productId,
            };

            _context.favoritProducts.Add(favoritProduct);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [Authorize]
        [HttpPost("RemoveFromFavorites")]
        public async Task<IActionResult> RemoveFromFavorites(int productId)
        {
            var userIdClime = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClime == null)
            {
                return Unauthorized("Користувач не авторизований");
            }
            int userId = int.Parse(userIdClime);

            var product = await _context.products.FindAsync(productId);

            if (product == null)
            {
                return BadRequest("Неправельне ID товару");
            }

            var favoritProduct = await _context.favoritProducts.FirstOrDefaultAsync(fp => fp.UserId == userId&& fp.ProductId == productId);

            if (favoritProduct == null)
            {
                return NotFound("Цей товар не знайдено в обраному");
            }

            _context.favoritProducts.Remove(favoritProduct);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Успішно видалено з улюбленого"});
        }
        [Authorize]
        [HttpGet("GetFavoritesByUser")]
        public async Task<IActionResult> GetFavoritesByUser()
        {
            var userIdClime = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClime == null)
            {
                return Unauthorized("Користувач не авторизований");
            }
            int userId = int.Parse(userIdClime);

            var favoritProducts = await _context.favoritProducts.Include(fp=> fp.Product).Where(fp=> fp.UserId == userId).ToListAsync();

            if (!favoritProducts.Any())
            {
                return NotFound("Немає улюблених продуктів");
            }
            return Ok(favoritProducts);

        }
    }
}
