using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models.User;
using MyShopProjectBackend.Servises.Interface;
using NLog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ILogger = NLog.ILogger;

namespace MyShopProjectBackend.Servises
{
    public class AccountServise : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public AccountServise(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<UserDto?> GetCurrentUserAsync()
        {
            _logger.Info($"{nameof(GetCurrentUserAsync)}: Виклик методу");
            var user = await _userManager.GetUserAsync(_signInManager.Context.User);
            if (user == null)
            {
                _logger.Error($"{nameof(GetCurrentUserAsync)}: Поточний користувач не знайдений");
                throw new AuthorizationException("Користувач не знайдений");
            }

            var userDto = new UserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
            };
            _logger.Info($"{nameof(GetCurrentUserAsync)}: Користувач успішно отриманий та перетворений у DTO");
            return userDto;
        }

        public async Task<string> LoginAsync(LoginModel loginModel)
        {
            _logger.Info($"{nameof(LoginAsync)}:Виклик методу для користувача {loginModel.Username}");

            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user == null)
            {
                _logger.Info($"{nameof(LoginAsync)}: Користувача {loginModel.Username} не знайдено");
                throw new AuthorizationException("Користувач не знайдений");
            }

            var passwordValid = await _userManager.CheckPasswordAsync(user, loginModel.Password);
            if (!passwordValid)
            {
                _logger.Info($"{nameof(LoginAsync)}: Невірний пароль для {loginModel.Username}");
                throw new AuthorizationException("Невірний пароль");
            }

            var tokenString = await GenerateJwtTokenAsync(user);
            _logger.Info($"{nameof(LoginAsync)}: Успішний вхід користувача {loginModel.Username}");
            return tokenString;
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            _logger.Info($"{nameof(GenerateJwtTokenAsync)}: Виклик методу");
            _logger.Info($"{nameof(GenerateJwtTokenAsync)}: Створення токена для {user.UserName}");

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                _logger.Error($"{nameof(GenerateJwtTokenAsync)}: Неправильна конфігурація JWT");
                throw new BadRequestException("Неправильна конфігурація JWT.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName)
    };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            _logger.Info($"{nameof(GenerateJwtTokenAsync)}: Зібрано {claims.Count} клеймів для JWT");

            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = issuer,
                Audience = audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            _logger.Info($"{nameof(GenerateJwtTokenAsync)}: JWT токен успішно створено для {user.UserName}");
            return tokenString;
        }

        public async Task<string> RegisterUserAsync(RegisterUserModel model, string role)
        {
            _logger.Info($"{nameof(RegisterUserAsync)}:Виклик методу та Реєстрація користувача {model.Username} з роллю {role}");

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                _logger.Info($"{nameof(RegisterUserAsync)}: Користувач {model.Username} вже існує");
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
                _logger.Info($"{nameof(RegisterUserAsync)}: Помилка створення користувача {model.Username}: {errors}");
                throw new BadRequestException($"Помилка при реєстрації користувача: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                _logger.Info($"{nameof(RegisterUserAsync)}: Помилка додавання ролі {role} користувачу {model.Username}: {errors}");
                throw new BadRequestException($"Помилка при додаванні ролі {role} користувачу: {errors}");
            }

            var token = await LoginAsync(new LoginModel { Username = model.Username, Password = model.Password });
            _logger.Info($"{nameof(RegisterUserAsync)}: Реєстрація успішна, токен створено для {model.Username}");
            return token;
        }
    }
}
