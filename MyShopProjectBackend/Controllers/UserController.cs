using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Models.User;
using MyShopProjectBackend.Servises.Interface;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("status")]
        public IActionResult Get() => Ok("API працює");//готово

        [HttpGet("by-username")]
        public async Task<IActionResult> GetUserByName(string userName)//готово
        {
            var result = await _userService.GetUserByNameAsync(userName);
            return Ok(result);
        }
        [Authorize]
        [HttpGet("curent")]
        public async Task<IActionResult> GetCurrentUser()//готово
        {
            var result = await _userService.GetUserByIdAsync(User.GetUserId());
            return Ok(result);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)//готово
        {
            await _userService.UpdateUserAsync(model, User.GetUserId());

            return Ok(new { message = "Користувача оновлено успішно" });
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()//готово
        {
            var result = await _userService.GetAllUsersAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()//готово
        {
            await _userService.DeleteUserAsync(User.GetUserId());
            return Ok(new { message = "Користувача видалено успішно" });
        }

    }
}
