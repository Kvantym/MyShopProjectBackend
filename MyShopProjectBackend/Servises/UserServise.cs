using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;

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
        public async Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return (false,"Користувача не знайдено");
            }
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                // Тут можна повернути помилки result.Errors
                return (false, "Не вдалося видалити користувача");
            }
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage, List<UserDto> Users)> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            if (users == null || !users.Any())
            {
                return (false, "Користувачів не знайдено", new List<UserDto>());
            }

            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                   
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "No role" // Якщо потрібно — можна повернути всі ролі, а не одну
                });
            }

            return (true, null, userDtos);
        }


        public async Task<(bool Success, string? ErrorMessage, ApplicationUser? user)> GetUserByIdAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return (false, "Користувача не знайдеено",null);
            }
            return (true, null,user);
          }

        public async Task<(bool Success, string? ErrorMessage)> UpdateUserAsync(UpdateUserModel model, string? oldPassword = null)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                return (false, "Користувача не знайдено");
            }

            user.UserName = model.Name;
            user.Email = model.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return (false, string.Join("; ", updateResult.Errors.Select(e => e.Description)));
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                if (oldPassword == null)
                {
                    // Якщо старий пароль не передано, можна скинути пароль адміністратором (не безпечно, якщо без підтвердження)
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetResult = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    if (!resetResult.Succeeded)
                    {
                        return (false, string.Join("; ", resetResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    // Зміна пароля з вказаним старим паролем
                    var changeResult = await _userManager.ChangePasswordAsync(user, oldPassword, model.Password);
                    if (!changeResult.Succeeded)
                    {
                        return (false, string.Join("; ", changeResult.Errors.Select(e => e.Description)));
                    }
                }
            }

            return (true, null);
        }

    }
}
