using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {

        private readonly AppDbConection _context;

        public AccountController(AppDbConection conection)
        {
            _context = conection;
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
            if(!ModelState.IsValid)// Перевірка моделі на валідність
            {
                return BadRequest(ModelState);// Перевірка моделі на валідність
            }
            var user = await _context.users.FirstOrDefaultAsync(u => u.UserName == loginModel.Username && u.Password == loginModel.Password);// Знайти користувача за ім'ям та паролем

            if (user == null)// Якщо користувач не знайдений
            {
                return Unauthorized("Invalid username or password");// Повернути статус 401 Unauthorized
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),  // Класифікаційні дані токена
                new Claim(ClaimTypes.Role, user.Role), // Роль користувача
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey1234567890!@#$%^&*()_+QWERTY")); // Ключ для підпису токена
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Створення об'єкта підпису токена

            var token = new JwtSecurityToken(
                issuer: "MyShopProjectBackend", // Видавець токена
                audience: "MyShopProjectFron", // Аудиторія токена
                claims: claims, // Класифікаційні дані токена
                expires: DateTime.Now.AddMinutes(30), // Термін дії токена
                signingCredentials: creds // Підпис токена
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token); // Генерація токена у форматі рядка

           return Ok(new {token= tokenString, user.Role } );// Повернути статус 200 OK з токеном у форматі рядка
        }

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] LoginModel loginMode)
        {
           return await RegisterUser(loginMode, UserRole.Customer); // Виклик методу для реєстрації покупця
        }
        [HttpPost("RegisterSeller")]
        public async Task<IActionResult> RegisterSeller([FromBody] LoginModel loginMode)
        {
           return await RegisterUser(loginMode, UserRole.Seller); // Виклик методу для реєстрації продавця
        }

        private async Task<IActionResult> RegisterUser(LoginModel loginModel, string role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _context.users.FirstOrDefaultAsync(u => u.UserName == loginModel.Username || u.Email == loginModel.Email);
            if (existingUser != null)
            {
                return BadRequest("User with this username or email already exists.");
            }
            var newUser = new User
            {
                UserName = loginModel.Username,
                Password = loginModel.Password,
                Email = loginModel.Email,
                Role = role
            };

            await _context.users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Користувача успішно зареєстровано", userId = newUser.Id });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.SignOutAsync();
            return Ok(new { message = "Користувач вийшов із системи" });
        }


        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser() 
        {
            var userName = User.Identity?.Name; // Отримання імені користувача з контексту аутентифікації
            
            if (string.IsNullOrEmpty(userName)) // Перевірка, чи ім'я користувача не є порожнім
            {
                return Unauthorized("User is not authenticated"); // Повернути статус 401 Unauthorized, якщо користувач не аутентифікований
            }

            var user = await _context.users.FirstOrDefaultAsync(u => u.UserName == userName); // Знайти користувача за іменем
            if (user == null) // Перевірка, чи користувач існує
            {
                return NotFound("User not found"); // Повернути статус 404 Not Found, якщо користувач не знайдений
            }

            return Ok(new { user.Id,user.UserName, user.Email, user.Role});
        }

    }
}
