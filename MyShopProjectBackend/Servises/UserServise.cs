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

        public UserServise(AppDbConection context)
        {
            _context = context;
        }
        public async Task<(bool Success, string? ErrorMessage)> DeleteUserAsync(int userId)
        {
            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return (false,"Користувача не знайдено");
            }
            _context.users.Remove(user);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage, List<UserDto> Users)> GetAllUsersAsync()
        {
            var users = await _context.users.ToListAsync();

            if (users == null)
            {
                return (false,"Користувачів не знайдено ", new List<UserDto>());
            }

            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Role = u.Role
            }).ToList();

            return (true, null, userDtos);
        }

        public async Task<(bool Success, string? ErrorMessage, User? user)> GetUserByIdAsync(int userId)
        {
            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return (false, "Користувача не знайдеено",null);
            }
            return (true, null,user);
          }

        public async Task<(bool Success, string? ErrorMessage)> UpdateUserAsync(UpdateUserModel model)
        {
            var user = await _context.users.FindAsync(model.UserId);

            if (user == null)
            {
                return (false,"Користувача не знайдеено");
            }

            user.UserName = model.Name;
            user.Email = model.Email;
            user.Password = model.Password;

            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}
