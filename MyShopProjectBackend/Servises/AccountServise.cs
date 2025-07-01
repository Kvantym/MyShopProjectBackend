using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyShopProjectBackend.Db;
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
        private readonly AppDbConection _context;
        private readonly IConfiguration _configuration; // Додано для доступу до конфігурації, якщо потрібно

        public AccountServise(AppDbConection conection, IConfiguration configuration)
        {
            _context = conection;
            _configuration = configuration; // Зберігаємо конфігурацію для подальшого використання

        }

        public async Task<(bool Success, string? ErrorMessage, UserDto? userDto)> GetCurrentUserAsync(int userId)
        {
            var user = await _context.users.FindAsync(userId);

            if (user == null)
            {
                return (false, "Користувача не знайдено", null);
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role
            };
            return (true, null, userDto); // Повернути статус успіху та DTO користувача
        }

        public async Task<(bool Success, string? token, string? ErrorMessage)> LoginAsync(LoginModel loginModel)
        {
           var user = await _context.users.FirstOrDefaultAsync(u => u.UserName == loginModel.Username && u.Password == loginModel.Password);
            if (user == null)
            {
                return (false,null, "Неправельний логін чи пароль");
            }
           
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),  // Класифікаційні дані токена
                new Claim(ClaimTypes.Role, user.Role), // Роль користувача
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey1234567890!@#$%^&*()_+QWERTY")); // Ключ для підпису токена
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // Створення об'єкта підпису токена

            var token = new JwtSecurityToken(
                issuer: "MyShopProjectBackend", // Видавець токена
                audience: "MyShopProjectFron", // Аудиторія токена
                claims: claims, // Класифікаційні дані токена
                expires: DateTime.Now.AddMinutes(30), // Термін дії токена
                signingCredentials: creds // Підпис токена
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token); // Генерація токена у форматі рядка

            return (true, tokenString,null); // Повернути статус успіху та токен у форматі рядка

        }

        public async Task<(bool Success, string? ErrorMessage, int? UserId)> RegisterUserAsync(LoginModel loginModel, string role)
        {
            var existingUser = await _context.users.FirstOrDefaultAsync(u => u.UserName == loginModel.Username || u.Email == loginModel.Email);
            if (existingUser != null)
            {
                return (false, "Користувач з таким імям або емайлом існує", null);
            }
            var newUser = new User
            {
                UserName = loginModel.Username,
                Password = loginModel.Password,
                Email = loginModel.Email,
                Role = role
            };

            await _context.users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return (true, null, newUser.Id); // Повернути статус успіху та ідентифікатор нового користувача
        }
    }
}
