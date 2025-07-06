using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Login;
using MyShopProjectBackend.ViewModels.Register;
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

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            var user = await _userManager.GetUserAsync(_signInManager.Context.User);
            if (user == null) {
               throw new AuthorizationException("Користувач не знайдений");
            }
            var userDto = new UserDto
            {
               
                UserName = user.UserName,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
            };
            return userDto;
        }


        public async Task<string> LoginAsync(LoginModel loginModel)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user == null) 
            {
              throw new AuthorizationException("Користувач не знайдений");
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginModel.Password);
            if (!passwordValid)
            {
                throw new AuthorizationException("Невірний пароль");
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

            var tokenString = await GenerateJwtTokenAsync(user);

            return tokenString;
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            foreach(var role in roles)
            {
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

            return tokenString;
        }

        public async Task<string> RegisterUserAsync(RegisterUserModel model, string role)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null) 
            {
               throw new RegistrationException("Користувач з таким іменем вже існує");
            }
            user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email
                
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Помилка при реєстрації користувача:{errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded) 
            {
                var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                throw new BadRequestException($"Помилка при додаванні ролі {role} до користувача:{errors}");
            }

            LoginModel loginModel = new LoginModel
            {
                Username = model.Username,
                Password = model.Password
            };

            var token = await LoginAsync(loginModel);

            return token;
        }

      

    }
}
