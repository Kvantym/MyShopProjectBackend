using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.User;
using MyShopProjectBackend.Domain.Responses;

namespace MyShopProjectBackend.Services.Interface
{
    public interface IUserService
    {
        public Task<ApplicationUser> GetUserByNameAsync(string userName);
        public Task UpdateUserAsync(UpdateUserRequest request, string userId, string? oldPassword = null);
        public Task<List<UserResponse>> GetAllUsersAsync();
        public Task DeleteUserAsync(string userId);
        public Task<ApplicationUser> GetUserByIdAsync(string userId);
        public Task<ApplicationUser> GetUserOrThrowAsyncId(string userId);
        public Task<ApplicationUser> GetUserOrThrowAsyncName(string userName);
        public Task<List<string>> EnsureUserHasRoleOrThrowAsync(ApplicationUser user, string requiredRole);
        public void EnsureSellerOwnsOrder(Order order, ApplicationUser user);
        public Task<List<ApplicationUser>> GetUsersByIdsAsync(List<string> userIds);
    }
}
