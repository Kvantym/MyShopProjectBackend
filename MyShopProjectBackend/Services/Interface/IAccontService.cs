using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models.User;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IAccountService
    {
        public Task<string> LoginAsync(LoginModel loginModel); 

        public Task<string> RegisterUserAsync(RegisterUserModel model, string role);

        public Task<UserDto?> GetCurrentUserAsync();
    }
}
