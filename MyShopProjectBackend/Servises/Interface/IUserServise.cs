using Microsoft.AspNetCore.Identity;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models.User;

namespace MyShopProjectBackend.Servises.Interface
{
    public interface IUserServise
    {
        public Task<ApplicationUser> GetUserByNameAsync(string userName);
        public Task UpdateUserAsync(UpdateUserModel model, string userId, string? oldPassword = null);
        public Task<List<UserDto>> GetAllUsersAsync();
        public Task DeleteUserAsync(string userId);
        public Task<ApplicationUser> GetUserByIdAsync(string userId);
        public Task<ApplicationUser> GetUserOrThrowAsyncId(string userId);
        public Task<ApplicationUser> GetUserOrThrowAsyncName(string userName);
        public Task<List<string>> EnsureUserHasRoleOrThrowAsync(ApplicationUser user, string requiredRole);
        public void EnsureSellerOwnsOrder(Order order, ApplicationUser user);
        public Task<List<ApplicationUser>> GetUsersByIdsAsync(List<string> userIds);


    }
}
 