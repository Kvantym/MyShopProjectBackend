using Microsoft.AspNetCore.Identity;

namespace MyShopProjectBackend.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Shop> Shops { get; set; } = new List<Shop>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<FavouriteProduct> FavoriteProducts { get; set; } = new List<FavouriteProduct>();
    }
}
