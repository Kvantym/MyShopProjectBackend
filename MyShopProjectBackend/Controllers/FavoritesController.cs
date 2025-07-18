using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Models.Favorit;
using MyShopProjectBackend.Servises.Interface;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/favorites")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteServises;

        public FavoritesController(IFavoriteService favoriteServises)
        {
            _favoriteServises = favoriteServises;
        }

        [HttpGet("status")]
        public IActionResult Get() => Ok("API працює");//готово

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToFavorites([FromBody]AddFavoritModel model)//готово
        {
            var userId = User.GetUserId();
            await _favoriteServises.AddToFavoritesAsync(model, userId);

            return Ok(new { message = "Товар успішно додано до обраного" });
        }

        [Authorize]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromFavorites(int productId)//готово
        {
            var userId = User.GetUserId();
            await  _favoriteServises.RemoveFromFavoritesAsync(productId, userId);
            
            return Ok(new { message = "Товар успішно видалено з обраного" });
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetFavorites()//готово
        {
            var result = await _favoriteServises.GetFavoritesAsync(User.GetUserId());
            return Ok(result);
        }
    }
}
