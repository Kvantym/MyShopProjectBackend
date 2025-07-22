
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Domain.Entities;

namespace MyShopProjectBackend.Infrastructure
{
    public class AppDbConect : IdentityDbContext<ApplicationUser>
    {
        public AppDbConect(DbContextOptions<AppDbConect> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; } // Таблиця продуктів
        public DbSet<FavouriteProduct> FavoritProducts { get; set; }  // Таблиця улюблених продуктів
        public DbSet<Order> Orders { get; set; } // Таблиця замовлень
        public DbSet<OrderItem> OrderItems { get; set; } // Таблиця товарів в замовленнях
        public DbSet<ProductReview> ProductReviews { get; set; } // Таблиця відгуків на продукти
        public DbSet<Shop> Shops { get; set; } // Таблиця магазинів
        public DbSet<ShopOrder> ShopOrders { get; set; } // Таблиця замовлень магазинів
        public DbSet<Cart> Carts { get; set; } // Таблиця кошиків
        public DbSet<CartItem> CartItems { get; set; } // Таблиця товарів в кошиках


    }
}
