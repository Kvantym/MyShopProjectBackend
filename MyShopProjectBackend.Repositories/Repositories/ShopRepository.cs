using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Infrastructure;
using MyShopProjectBackend.Repositories.Interfaces;

namespace MyShopProjectBackend.Repositories.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly AppDbConect _context;
        public ShopRepository(AppDbConect context)
        {
            _context = context;
        }

        public async Task AddAsync(Shop shop)
        {
           _context.Shops.Add(shop);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Shop shop)
        {
            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Shop>> GetAllByOwnerAsync(ApplicationUser seller)
        {
            return await _context.Shops.Where(s => s.OwnerId == seller.Id).ToListAsync();
        }

        public async Task<Shop?> GetByIdAsync(int shopId)
        {
            return await _context.Shops.FirstOrDefaultAsync(s => s.Id == shopId);
        }

        public async Task UpdateAsync(Shop shop)
        {
            _context.Shops.Update(shop);
            await _context.SaveChangesAsync();
        }
    }
}
