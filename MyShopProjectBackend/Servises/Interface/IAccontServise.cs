using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models.User;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IAccountService
    {
        public Task<string> LoginAsync(LoginModel loginModel); // Асинхронний метод для входу користувача

        public Task<string> RegisterUserAsync(RegisterUserModel model, string role); // Асинхронний метод для реєстрації користувача з вказаною роллю

        public Task<UserDto?> GetCurrentUserAsync();
    }
}
