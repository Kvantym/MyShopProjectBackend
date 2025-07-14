using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace MyShopProjectBackend.Entities
{
    public class ApplicationUser : IdentityUser 
    {
        public ICollection<Shop> Shops { get; set; } = new List<Shop>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<FavouriteProduct> FavoritProducts { get; set; } = new List<FavouriteProduct>();
    }
}
