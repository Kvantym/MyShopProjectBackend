using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyShopProjectBackend.Servises
{
    public class AccountServise : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountServise(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<(bool Success, string? ErrorMessage, UserDto? userDto)> GetCurrentUserAsync()
        {
            var user = await _userManager.GetUserAsync(_signInManager.Context.User);
            if (user == null) {
                return (false, "Користувач не знайдений", null);
            }
           
            var userDto = new UserDto
            {
               
                UserName = user.UserName,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()?? "Немає ролі",
            };

            return (true, null, userDto);
        }


        public async Task<(bool Success, string? token, string? ErrorMessage)> LoginAsync(LoginModel loginModel)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user == null) {
                return (false, null, "Користувача не знайдено");
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginModel.Password);
            if (!passwordValid)
            { 
                return (false, null, "Невірний пароль");
            }

            var roles = await _userManager.GetRolesAsync(user);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id), 
                new Claim(ClaimTypes.Name, user.UserName)
            };

            foreach (var role in roles) {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

                Issuer = issuer,   
                Audience = audience
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return (true, tokenString, null);
        }


        public async Task<(bool Success, string? ErrorMessage)> RegisterUserAsync(RegisterUserModel model, string role)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null) {
                return (false, "Користувач з таким іменем вже існує");
            }
            user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
                
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) {
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded) {
                return (false, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }
            return (true, null);
        }

    }
}
