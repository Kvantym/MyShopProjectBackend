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
        }
    }
}
