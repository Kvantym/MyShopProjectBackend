using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Login;
using MyShopProjectBackend.ViewModels.Register;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;


        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;

        }

        [HttpGet("Get")]
        public IActionResult Get() => Ok("API працює");

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var result = await _accountService.LoginAsync(loginModel);
            return Ok(new { token = result });
        }

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterUserModel model)
        {
            var result = await _accountService.RegisterUserAsync(model, UserRole.Customer);
            return Ok(result);
        }

        [HttpPost("RegisterSeller")]
        public async Task<IActionResult> RegisterSeller([FromBody] RegisterUserModel model)
        {
            var result = await _accountService.RegisterUserAsync(model, UserRole.Seller);
            return Ok(result);
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
            var result = await _accountService.GetCurrentUserAsync();
            return Ok(result);
        }
    }
}
