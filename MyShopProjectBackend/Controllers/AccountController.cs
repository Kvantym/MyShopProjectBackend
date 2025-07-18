using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Models.User;
using MyShopProjectBackend.Servises.Interface;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;


        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("status")]
        public IActionResult Get() => Ok("API працює");//готово

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)//готово
        {
            var result = await _accountService.LoginAsync(loginModel);
            return Ok(new { token = result });
        }

        [HttpPost("register-customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterUserModel model)//готово
        {
            var result = await _accountService.RegisterUserAsync(model, UserRole.Customer);
            return Ok(result);
        }

        [HttpPost("register-seller")]
        public async Task<IActionResult> RegisterSeller([FromBody] RegisterUserModel model)//готово
        {
            var result = await _accountService.RegisterUserAsync(model, UserRole.Seller);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()//готово
        {
            await HttpContext.SignOutAsync();
            return Ok(new { message = "Користувач вийшов із системи" });
        }

        [Authorize]
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()//готово
        {
            var result = await _accountService.GetCurrentUserAsync();
            return Ok(result);
        }
    }
}
