using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Models;

namespace MyShopProjectBackend.Db
{
    public class AppDbConection : IdentityDbContext<ApplicationUser>
    {
        public AppDbConection(DbContextOptions<AppDbConection> options) : base(options)
        {
        }
        public DbSet<Product> products { get; set; } // Таблиця продуктів
        public DbSet<FavoritProduct> favoritProducts  { get; set; }  // Таблиця улюблених продуктів
        public DbSet<Order> orders  { get; set; } // Таблиця замовлень
        public DbSet<OrderItem> orderItems  { get; set; } // Таблиця товарів в замовленнях
        public DbSet<ProductReview> productReviews  { get; set; } // Таблиця відгуків на продукти
        public DbSet<Shop> shops  { get; set; } // Таблиця магазинів
        public DbSet<ShopOrder> shopOrders  { get; set; } // Таблиця замовлень магазинів
        public DbSet<Cart> carts { get; set; } // Таблиця кошиків
        public DbSet<CartItem> cartItems { get; set; } // Таблиця товарів в кошиках
       

    }
}
