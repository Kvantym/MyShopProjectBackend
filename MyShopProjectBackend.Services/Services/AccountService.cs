using Microsoft.AspNetCore.Identity;
using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.Account;
using MyShopProjectBackend.Domain.Responses;
using MyShopProjectBackend.Services.Exceptions;
using MyShopProjectBackend.Services.Interface;
using System.Data;

namespace MyShopProjectBackend.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }

        public async Task<UserResponse> GetCurrentUserAsync()
        {
            var user = await _userManager.GetUserAsync(_signInManager.Context.User);
            if (user == null) 
            {
                throw new AuthorizationException("Користувача не знайдено");
            }
            var userrRespons = new UserResponse
            {
                UserName = user.UserName,
                Email = user.Email,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
            };
            return userrRespons;
        }

        public async Task<string> LoginAsync(LoginRequest request)
        {
            var user = await _userService.GetUserOrThrowAsyncName(request.Username);

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
            {
                throw new AuthorizationException("Невірний пароль");
            }

            var tokenString = "1";// = await GenerateJwtTokenAsync(user); //тут створюється токен і я думаю що треба додати якійсь сервіс для генерації токенів
            return tokenString;
        }
       

        public async Task<string> RegisterUserAsync(RegisterUserRequest request)
        {
            var user = await _userService.GetUserOrThrowAsyncName(request.Username);
            if (user != null)
            {
                throw new BadRequestException("Користувач з таким іменем вже існує.");
            }

            user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new BadRequestException($"Помилка при реєстрації користувача: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));

                throw new BadRequestException($"Помилка при додаванні ролі користувачу: {errors}");
            }

            //var token = await LoginAsync(new LoginModel { Username = model.Username, Password = model.Password });
            //return token;
            return "1"; //тут треба повернути токен але в мене немає сервісу для генерації токенів
        }
    }
}
