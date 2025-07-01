using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IAccountService
    {
        public Task<(bool Success, string? token, string? ErrorMessage)> LoginAsync(LoginModel loginModel); // Асинхронний метод для входу користувача

        public Task<(bool Success, string? ErrorMessage,int? UserId)> RegisterUserAsync(LoginModel loginModel, string role); // Асинхронний метод для реєстрації користувача з вказаною роллю

        public Task<(bool Success, string? ErrorMessage, UserDto? userDto)> GetCurrentUserAsync(int userId); // Асинхронний метод для отримання поточного користувача
    }
}
