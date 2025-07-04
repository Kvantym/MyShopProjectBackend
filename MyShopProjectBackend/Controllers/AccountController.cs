using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyShopProjectBackend.Servises.Interface;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {


        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountServise)
        {
            _accountService = accountServise;
        }
        // GET: AccountController
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet("Get")]
        public IActionResult Get() => Ok("API працює");

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)// Перевірка моделі на валідність
            {
                return BadRequest(ModelState);// Перевірка моделі на валідність
            }

            var result = await _accountService.LoginAsync(loginModel); // Виклик методу для входу користувача
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо вхід не вдався
            }
            return Ok(new { token = result.token });
        }

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] LoginModel loginMode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountService.RegisterUserAsync(loginMode, UserRole.Customer); // Виклик методу для реєстрації продавця

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо реєстрація не вдалась
            }

            return Ok(new { message = "Користувача успішно зареєстровано", userId = result.UserId });
        }
        [HttpPost("RegisterSeller")]
        public async Task<IActionResult> RegisterSeller([FromBody] LoginModel loginMode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _accountService.RegisterUserAsync(loginMode, UserRole.Seller); // Виклик методу для реєстрації продавця

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо реєстрація не вдалась
            }

            return Ok(new { message = "Користувача успішно зареєстровано", userId = result.UserId });
        }



        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok(new { message = "Користувач вийшов із системи" });
        }


        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Отримання ідентифікатора користувача з токена
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found");
            }
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID");
            }

            var result = await _accountService.GetCurrentUserAsync(userId); // Виклик методу для отримання поточного користувача

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо користувача не знайдено
            }
            return Ok(new { result.userDto }); // Повернення DTO користувача

        }

    }
}
