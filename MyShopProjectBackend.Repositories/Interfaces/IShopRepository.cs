using MyShopProjectBackend.Domain.Entities;

namespace MyShopProjectBackend.Repositories.Interfaces
{
    public interface IShopRepository
    {
        Task AddAsync(Shop shop);
        Task UpdateAsync(Shop shop);
        Task DeleteAsync(Shop shop);
        Task<List<Shop>> GetAllByOwnerAsync(ApplicationUser seller);
        Task<Shop?> GetByIdAsync(int shopId);
    }
}
