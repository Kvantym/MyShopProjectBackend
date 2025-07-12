using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Models.User;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IUserServise
    {
        public Task<ApplicationUser?> GetUserByNameAsync(string userName);
        public Task UpdateUserAsync(UpdateUserModel model, string userId, string? oldPassword = null);
        public Task<List<UserDto>> GetAllUsersAsync();
        public Task DeleteUserAsync(string userId);
        public Task<ApplicationUser?> GetUserByIdAsync(string userId);

    }
}
 