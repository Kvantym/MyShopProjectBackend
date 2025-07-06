using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Add;
using MyShopProjectBackend.ViewModels.Remove;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteServises _favoriteServises;

        public FavoritesController(IFavoriteServises favoriteServises)
        {
            _favoriteServises = favoriteServises;
        }

        // GET: FavoritesController
        [HttpGet]
        public ActionResult Index()
        {
            return Ok("FavoritesController Working");
        }
        [Authorize]
        [HttpPost("AddToFavorites")]
        public async Task<IActionResult> AddToFavorites(AddFavoritModel model)
        {
            model.UserId = User.GetUserId();
            var result =  _favoriteServises.AddToFavoritesAsync(model);
           
            return Ok(new { message = "Товар успішно додано до обраного" });
        }

        [Authorize]
        [HttpPost("RemoveFromFavorites")]
        public async Task<IActionResult> RemoveFromFavorites(RemoveFavoritModel model)
        {
            model.UserId = User.GetUserId();
            var result =  _favoriteServises.RemoveFromFavoritesAsync(model);
            
            return Ok(new { message = "Товар успішно видалено з обраного" });
        }
        [Authorize]
        [HttpGet("GetFavoritesByUser")]
        public async Task<IActionResult> GetFavoritesByUser()
        {
            var result = await _favoriteServises.GetFavoritesAsync(User.GetUserId());
            return Ok(result);
        }
    }
}
