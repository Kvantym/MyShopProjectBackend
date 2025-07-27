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

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        public async Task<ApplicationUser> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user;
        }

        public async Task<List<ApplicationUser>> GetUsersByIdsAsync(List<string> userIds)
        {
           var users = await _userManager.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
           return users;
        }

        public async Task UpdateUserAsync(ApplicationUser user)
        {
            
            var existingUser = await _userManager.FindByIdAsync(user.Id);

            existingUser.UserName = user.UserName;
            existingUser.Email = user.Email;

            await _userManager.UpdateAsync(existingUser);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        }
    }
}
