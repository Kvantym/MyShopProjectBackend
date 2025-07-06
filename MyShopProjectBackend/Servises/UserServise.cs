using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Servises
{
    public class UserServise : IUserServise
    {
        private readonly AppDbConection _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserServise(AppDbConection context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Користувача не знайдено");
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new BadRequestException("Не вдалося видалити користувача: " + string.Join("; ", result.Errors.Select(e => e.Description)));
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users == null || !users.Any())
            {
                throw new NotFoundException("Користувачів не знайдено");
            }

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault()
                });
            }

            return userDtos;
        }

        public async Task<ApplicationUser?> GetUserByNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new NotFoundException("Користувача не знайдено");
            }
            return user;
        }

        public async Task UpdateUserAsync(UpdateUserModel model, string? oldPassword = null)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Користувача не знайдено");
            }

            user.UserName = model.Name;
            user.Email = model.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new BadRequestException("Не вдалося оновити користувача: " + string.Join("; ", updateResult.Errors.Select(e => e.Description)));
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                if (oldPassword == null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    if (!resetResult.Succeeded)
                    {
                        throw new BadRequestException("Не вдалося оновити користувача: " + string.Join("; ", resetResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    var changeResult = await _userManager.ChangePasswordAsync(user, oldPassword, model.Password);
                    if (!changeResult.Succeeded)
                    {
                        throw new BadRequestException("Не вдалося оновити користувача: " + string.Join("; ", changeResult.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

    }
}
