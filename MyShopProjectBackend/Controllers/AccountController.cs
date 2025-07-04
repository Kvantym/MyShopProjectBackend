using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;

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
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(new { token = result.token });
        }

        [HttpPost("RegisterCustomer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterUserModel model)
        {
           var result = await _accountService.RegisterUserAsync(model, UserRole.Customer);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(new { message = "Користувач зареєстрований" });
        }

    

        [HttpPost("RegisterSeller")]
        public async Task<IActionResult> RegisterSeller([FromBody] RegisterUserModel loginModel)
        {
            var result = await _accountService.RegisterUserAsync(loginModel, UserRole.Seller);
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(new { message = "Продавець зареєстрований" });

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
            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.userDto);

        } 
    }
}
