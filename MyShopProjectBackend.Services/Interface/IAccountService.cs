using MyShopProjectBackend.Domain.Request.Account;
using MyShopProjectBackend.Domain.Responses;

namespace MyShopProjectBackend.Services.Interface
{
    public interface IAccountService
    {
        public Task<string> LoginAsync(LoginRequest request);

        public Task<string> RegisterUserAsync(RegisterUserRequest request);

        public Task<UserResponse> GetCurrentUserAsync();
    }
}
