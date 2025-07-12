using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models.User;
using MyShopProjectBackend.Servises.Interface;
using NLog;

namespace MyShopProjectBackend.Servises
{
    public class UserServise : IUserServise
    {
        private readonly AppDbConection _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private static readonly NLog.ILogger _logger = LogManager.GetCurrentClassLogger();

        public UserServise(AppDbConection context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task DeleteUserAsync(string userId)
        {
            _logger.Info($"{nameof(DeleteUserAsync)}: Виклик методу");
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.Warn($"{nameof(DeleteUserAsync)}: Користувача з ID {userId} не знайдено");
                    throw new NotFoundException("Користувача не знайдено");
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    _logger.Error($"{nameof(DeleteUserAsync)}: Помилка видалення користувача з ID {userId}: {errors}");
                    throw new BadRequestException("Не вдалося видалити користувача: " + errors);
                }

                await _context.SaveChangesAsync();
                _logger.Info($"{nameof(DeleteUserAsync)}: Користувача з ID {userId} успішно видалено");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{nameof(DeleteUserAsync)}: Виняток при видаленні користувача з ID {userId}");
                throw;
            }
        }


        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            _logger.Info($"{nameof(GetAllUsersAsync)}: Виклик методу");
            var users = await _userManager.Users.ToListAsync();

            if (users == null || !users.Any())
            {
                _logger.Warn($"{nameof(GetAllUsersAsync)}: Користувачів не знайдено");
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

            _logger.Info($"{nameof(GetAllUsersAsync)}: Отримано {userDtos.Count} користувачів, {nameof(GetAllUsersAsync)}: Виконано успішно");
            return userDtos;
        }

        public async Task<ApplicationUser?> GetUserByNameAsync(string userName)
        {
            _logger.Info($"{nameof(GetUserByNameAsync)}: Виклик методу");
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                _logger.Warn($"{nameof(GetUserByNameAsync)}: Користувача з іменем {userName} не знайдено");
                throw new NotFoundException("Користувача не знайдено");
            }

            _logger.Info($"{nameof(GetUserByNameAsync)}: Користувача {userName} знайдено, {nameof(GetUserByNameAsync)}: Виконано успішно");
            return user;
        }

        public async Task UpdateUserAsync(UpdateUserModel model, string userId, string? oldPassword = null)
        {
            _logger.Info($"{nameof(UpdateUserAsync)}: Виклик методу");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Warn($"{nameof(UpdateUserAsync)}: Користувача з ID {userId} не знайдено");
                throw new NotFoundException("Користувача не знайдено");
            }

            user.UserName = model.Name;
            user.Email = model.Email;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                _logger.Error($"{nameof(UpdateUserAsync)}: Помилка оновлення користувача: {errors}");
                throw new BadRequestException("Не вдалося оновити користувача: " + errors);
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                if (oldPassword == null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    if (!resetResult.Succeeded)
                    {
                        var errors = string.Join("; ", resetResult.Errors.Select(e => e.Description));
                        _logger.Error($"{nameof(UpdateUserAsync)}: Помилка скидання паролю: {errors}");
                        throw new BadRequestException("Не вдалося оновити користувача: " + errors);
                    }
                }
                else
                {
                    var changeResult = await _userManager.ChangePasswordAsync(user, oldPassword, model.Password);
                    if (!changeResult.Succeeded)
                    {
                        var errors = string.Join("; ", changeResult.Errors.Select(e => e.Description));
                        _logger.Error($"{nameof(UpdateUserAsync)}: Помилка зміни паролю: {errors}");
                        throw new BadRequestException("Не вдалося оновити користувача: " + errors);
                    }
                }
            }

            _logger.Info($"{nameof(UpdateUserAsync)}: Користувач {user.UserName} успішно оновлений, {nameof(UpdateUserAsync)}: Виконано успішно");
        }
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.Warn($"{nameof(GetUserByIdAsync)}: Користувача з ID {userId} не знайдено");
                throw new NotFoundException("Користувача не знайдено");
            }
            _logger.Info($"{nameof(GetUserByIdAsync)}: Користувача {user.UserName} знайдено, {nameof(GetUserByIdAsync)}: Виконано успішно");
            return user;
        }
    }
}
