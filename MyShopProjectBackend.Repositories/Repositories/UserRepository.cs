using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.User;
using MyShopProjectBackend.Domain.Responses;
using MyShopProjectBackend.Repositories.Interfaces;

namespace MyShopProjectBackend.Repositories.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
          _userManager = userManager;
        }

        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.DeleteAsync(user);
        }

        public Task<List<UserResponse>> GetAllAsync()
        {
           var users = _userManager.Users.Select(u => new UserResponse
            {
                UserName = u.UserName,
                Email = u.Email,
                Role = _userManager.GetRolesAsync(u).Result.FirstOrDefault()
            }).ToListAsync();
            return users;
        }

        public Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            var user = _userManager.FindByIdAsync(userId);
            return user;
        }

        public Task<ApplicationUser> GetUserByNameAsync(GetUserByNameRequest request)
        {
            var user = _userManager.FindByNameAsync(request.Username);
            return user;
        }

        public Task<List<ApplicationUser>> GetUsersByIdsAsync(List<string> userIds)
        {
           var users = _userManager.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
           return users;
        }

        public async Task UpdateUserAsync(UpdateUserRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);

            user.UserName = request.Username;
            user.Email = request.Email;

            await _userManager.UpdateAsync(user);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, request.Password);
        }
    }
}
