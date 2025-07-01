using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IUserServise
    {
        public Task<(bool Success, string? ErrorMessage, User? user)> GetUserByIdAsync(int userId);
        public Task<(bool Success, string? ErrorMessage)> UpdateUserAsync(UpdateUserModel model);
        public Task<(bool Success, string? ErrorMessage, List<UserDto> Users)> GetAllUsersAsync();
        public Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(int userId);

    }
}
 