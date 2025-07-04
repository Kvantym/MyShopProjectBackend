using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : Controller
    {
        private readonly AppDbConection _context;
        private readonly IFavoriteServises _favoriteServises;

        public FavoritesController(AppDbConection conection, IFavoriteServises favoriteServises)
        {
            _context = conection;
            _favoriteServises = favoriteServises;
        }

        // GET: FavoritesController
        [HttpGet]
        public ActionResult Index()
        {
            return Ok();
        }
        [Authorize]
        [HttpPost("AddToFavorites")]
        public async Task<IActionResult> AddToFavorites(AddFavoritModel model)
        {
            var userIdClime = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (userIdClime == null) {
                return Unauthorized("Користувач не авторизований");
            }
            model.UserId = int.Parse(userIdClime);

           var result = await _favoriteServises.AddToFavoritesAsync(model);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(new { message = "Товар успішно додано до обраного" });
        }

        [Authorize]
        [HttpPost("RemoveFromFavorites")]
        public async Task<IActionResult> RemoveFromFavorites(RemoveFavoritModel model)
        {
            var userIdClime = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClime == null)
            {
                return Unauthorized("Користувач не авторизований");
            }
            model.UserId = int.Parse(userIdClime);

            var result = await _favoriteServises.RemoveFromFavoritesAsync(model);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(new { message = "Товар успішно видалено з обраного" });
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

            if (!int.TryParse(userIdClime, out int userId))
            {
                return BadRequest("Некоректний ідентифікатор користувача");
            }

            var result = await _favoriteServises.GetFavoritesAsync(userId);
            if (!result.Success)
            {
                return NotFound(result.ErrorMessage);
            }
            return Ok(result.FavoriteProducts);

        }
    }
}
