using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Db;

namespace MyShopProjectBackend.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly AppDbConection _context;

        public FavoritesController(AppDbConection conection)
        {
            _context = conection;
        }

        // GET: FavoritesController
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddToFavorites(int productId)
        {
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFromFavorites(int productId)
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoritesByUser()
        {
            return Ok();

        }
    }
}
