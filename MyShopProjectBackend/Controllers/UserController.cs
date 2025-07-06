using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Update;
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

        [HttpGet]
        public ActionResult Index()
        {
            return Ok("User Controller is working");
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(string userName)
        {
            var result = await _userServise.GetUserByNameAsync(userName);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserModel model)
        {
            model.UserId = User.GetUserId();
            var result = _userServise.UpdateUserAsync(model);

            return Ok(new { message = "Користувача оновлено успішно" });
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userServise.GetAllUsersAsync();
            return Ok(result);
        }

        [Authorize]
        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser()
        {
            var resualt = _userServise.DeleteUserAsync(User.GetUserId());
            return Ok(new { message = "Користувача видалено успішно" });
        }

    }
}
