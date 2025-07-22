using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyShopProjectBackend.Repositories.Interfaces;
using MyShopProjectBackend.Repositories.Repositories;

namespace MyShopProjectBackend.Repositories.DI
{
    public static class DIRegister
    {
        public static void ConfigureRepositoriesDI(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IFavoriteRepository, FavoriteRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IShopRepository, ShopRepository>();
        }
    }
}
