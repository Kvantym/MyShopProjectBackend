using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbConection _context;

        public UserController(AppDbConection conection)
        {
            _context = conection;
        }

        // GET: UserController
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(int userId)
        {
            // Logic to get user by ID
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {
            return Ok();
        }

    }
}
