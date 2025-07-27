using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.User;
using MyShopProjectBackend.Domain.Responses;

namespace MyShopProjectBackend.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<ApplicationUser> GetUserByNameAsync(string userName);
        public Task UpdateUserAsync(ApplicationUser user);
        public Task<List<UserResponse>> GetAllAsync();
        public Task DeleteUserAsync(string userId);
        public Task<ApplicationUser> GetUserByIdAsync(string userId);
        public Task<List<ApplicationUser>> GetUsersByIdsAsync(List<string> userIds);
    }
}
