using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Models.User;
using MyShopProjectBackend.Servises.Interface;
using System.Security.Claims;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserServise _userServise;

        public UserController(IUserServise userServise)
        {
            _userServise = userServise;
        }

        [HttpGet("status")]
        public IActionResult Get() => Ok("API працює");//готово

        [HttpGet("by-username")]
        public async Task<IActionResult> GetUserByName(string userName)//готово
        {
            var result = await _userServise.GetUserByNameAsync(userName);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("curent")]
        public async Task<IActionResult> GetCurrentUser()//готово
        {
            var result = await _userServise.GetUserByIdAsync(User.GetUserId());
            return Ok(result);
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)//готово
        {
            await _userServise.UpdateUserAsync(model, User.GetUserId());

            return Ok(new { message = "Користувача оновлено успішно" });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()//готово
        {
            var result = await _userServise.GetAllUsersAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()//готово
        {
            await _userServise.DeleteUserAsync(User.GetUserId());
            return Ok(new { message = "Користувача видалено успішно" });
        }

    }
}
