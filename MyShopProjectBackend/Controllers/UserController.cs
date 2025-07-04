using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;
using System.Security.Claims;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly AppDbConection _context;
        private readonly IUserServise _userServise;

        public UserController(AppDbConection conection, IUserServise userServise)
        {
            _context = conection;
            _userServise = userServise;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var result = await _userServise.GetUserByIdAsync(userId);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.user);
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

            model.UserId = int.Parse(userIdClaim);
            var result = await _userServise.UpdateUserAsync(model);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(new { message = "Користувача оновлено успішно" });
        }
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {

            var result = await _userServise.GetAllUsersAsync();
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Users);
        }

        [Authorize]
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("Невірний ідентифікатор користувача");
            }
            var resualt = await _userServise.DeleteUserAsync(userId);
            if (!resualt.Success)
            {
                return BadRequest(resualt.ErrorMessage);
            }
            return Ok(new { message = "Користувача видалено успішно" });
        }

    }
}
