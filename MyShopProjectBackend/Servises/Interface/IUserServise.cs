using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IUserServise
    {
        public Task<ApplicationUser?> GetUserByNameAsync(string userName);
        public Task UpdateUserAsync(UpdateUserModel model, string? oldPassword = null);
        public Task<List<UserDto>> GetAllUsersAsync();
        public Task DeleteUserAsync(string userId);

    }
}
 