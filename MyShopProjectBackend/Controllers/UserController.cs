using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.ViewModels;
using System.Security.Claims;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly AppDbConection _context;

        public UserController(AppDbConection conection)
        {
            _context = conection;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(int userId)
        {
           var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Користувача не знайдеено");
            }
            return Ok(user);
        }
        [Authorize]
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("Користувач не авторизований");
            }

            int userId = int.Parse(userIdClaim);
            var user = await _context.users.FindAsync(userId);

            if (user == null)
            {
                return NotFound("Користувача не знайдеено");
            }

            user.UserName = model.Name;
            user.Email = model.Email;
            user.Password = model.Password;
          
            await _context.SaveChangesAsync();

            return Ok(new { message = "Користувача оновлено успішно" });
        }
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.users.ToListAsync();

            if(users == null)
            {
                return NotFound("Користувачів не знайдено ");
            }

            return Ok(users);

        }

    }
}
