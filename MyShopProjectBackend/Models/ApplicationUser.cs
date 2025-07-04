using Microsoft.AspNetCore.Identity;

namespace MyShopProjectBackend.Models
{
    public class ApplicationUser : IdentityUser // Клас користувача, який наслідує від IdentityUser для використання в ASP.NET Core Identity
    {
      

        public ICollection<Shop> Shops { get; set; } = new List<Shop>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<FavouriteProduct> FavoritProducts { get; set; } = new List<FavouriteProduct>();
    }
}
