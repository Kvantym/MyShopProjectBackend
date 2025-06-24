using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Controllers
{
    public class AccountController : Controller
    {

        private readonly AppDbConection _context;

        public AccountController(AppDbConection conection)
        {
            _context = conection;
        }
        // GET: AccountController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] LoginModel loginMode)
        {
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            return Ok();
        }

    }
}
