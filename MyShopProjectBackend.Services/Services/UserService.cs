using Microsoft.AspNetCore.Identity;
using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Request.User;
using MyShopProjectBackend.Domain.Responses;
using MyShopProjectBackend.Repositories.Interfaces;
using MyShopProjectBackend.Services.Exceptions;
using MyShopProjectBackend.Services.Interface;

namespace MyShopProjectBackend.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IUserRepository userRepository, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
        }
        public async Task DeleteUserAsync(string userId)
        {
            var user = await GetUserOrThrowAsyncId(userId);
            await _userRepository.DeleteUserAsync(userId);
        }

        public void EnsureSellerOwnsOrder(Order order, ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> EnsureUserHasRoleOrThrowAsync(ApplicationUser user, string requiredRole)
        {
            throw new NotImplementedException();
        }

        public async Task<List<UserResponse>> GetAllUsersAsync()
        {
            var  users = await _userRepository.GetAllAsync();
            return users.Select(u=> new UserResponse
            {
                UserName = u.UserName,
                Email = u.Email,
                Role = u.Role
            }).ToList();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await GetUserOrThrowAsyncId(userId);
        }

        public async Task<ApplicationUser> GetUserByNameAsync(string userName)
        {
            return await GetUserOrThrowAsyncName(userName);
        }

        public async Task<ApplicationUser> GetUserOrThrowAsyncId(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            return user;
        }

        public Task<ApplicationUser> GetUserOrThrowAsyncName(string userName)
        {
            var user = _userRepository.GetUserByNameAsync(userName);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            return user;
        }
        //зараз не треба , але може знадобитись в майбутньому
        public Task<List<ApplicationUser>> GetUsersByIdsAsync(List<string> userIds)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateUserAsync(UpdateUserRequest request, string userId, string? oldPassword = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("Користувача не знайдено.");
            }

            if (!string.IsNullOrEmpty(request.Password))
            {
                if (string.IsNullOrEmpty(oldPassword))
                {
                    throw new BadRequestException("Старий пароль обов'язковий для зміни пароля.");
                }

                var result = await _userManager.ChangePasswordAsync(user, oldPassword, request.Password);
                if (!result.Succeeded)
                {
                    throw new BadRequestException("Не вдалося змінити пароль: " + string.Join("; ", result.Errors.Select(e => e.Description)));
                }
            }

            user.UserName = request.Username;
            user.Email = request.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new BadRequestException("Не вдалося оновити користувача: " + string.Join("; ", updateResult.Errors.Select(e => e.Description)));
            }
        }

    }
}
