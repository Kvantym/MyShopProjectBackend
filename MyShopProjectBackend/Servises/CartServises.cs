using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels.Add;
using MyShopProjectBackend.ViewModels.Remove;
using MyShopProjectBackend.ViewModels.Update;

namespace MyShopProjectBackend.Servises
{
    public class CartServises : ICartServises
    {
        private readonly AppDbConection _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartServises(AppDbConection context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AddToCartAsync(AddToCartModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
               throw new AuthorizationException("Користувач не знайдений");
            }
            var product = await _context.products.FindAsync(model.ProductId);
            if (product == null)
            {
              throw new NotFoundException("Товар не знайдено");
            }
            if (product.Quantity < model.Quantity)
            {
               throw new BadRequestException("Недостатньо товару на складі");
            }
            if (model.Quantity <= 0)
            {
               throw new BadRequestException("Кількість товару має бути більше нуля");
            }

            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == model.UserId);
            if (cart == null)
            {
                cart = new Models.Cart
                {
                    UserId = model.UserId,
                    Items = new List<Models.CartItem>()
                };
                await _context.carts.AddAsync(cart);
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == model.ProductId);
            if (cartItem == null)
            {
                cart.Items.Add(new Models.CartItem
                {
                    ProductId = model.ProductId,
                    Quantity = model.Quantity,
                });
            }
            else
            {
                cartItem.Quantity += model.Quantity;  
            }
            await _context.SaveChangesAsync(); 
        }

        public async Task CheckoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new NotFoundException("Користувач не знайдений");
            }

            var cart = await _context.carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                throw new NotFoundException("Кошик порожній або не знайдено");
            }

            foreach (var item in cart.Items)
            {
                if (item.Product == null)
                {
                    throw new NotFoundException($"Товар з ID {item.ProductId} не знайдено в базі даних");
                }

                if (item.Product.Quantity < item.Quantity)
                {
                    throw new NotFoundException($"Недостатньо товару {item.Product.Name} не знайдено в базі даних");
                }
            }

            var order = new Models.Order
            {
                BuyerId = userId,
                Status = ShopOrderStatus.Confirmed.ToString(),
                OrderItems = new List<Models.OrderItem>()
            };

            foreach (var item in cart.Items)
            {

                item.Product.Quantity -= item.Quantity;

                order.OrderItems.Add(new Models.OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                });
            }

            _context.orders.Add(order);
            _context.cartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Користувач не знайдений");
            }
            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId); 
            if (cart == null)
            {
                throw new NotFoundException("Кошик не знайдено");
            }
            _context.cartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();
        }

        public async Task<CartDto?> GetCartAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new AuthorizationException("Користувач не знайдений");
            }
            var cart = await _context.carts.SingleOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                throw new NotFoundException("Кошик не знайдено");
            }

            var carItems = await _context.cartItems
                .Include(ci => ci.Product) 
                .Where(ci => ci.CartId == cart.Id) 
                .ToListAsync(); 

            var CartDto = new DTO.CartDto
            {
                UserId = cart.UserId,
                Id = cart.Id,
                Items = carItems.Select(c => new DTO.CartItemDto
                {
                    Id = c.Id,
                    Quantity = c.Quantity,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    ProductPrice = c.Product.Price
                }).ToList()
            };

           return CartDto;
        }

        public async Task RemoveFromCartAsync(RemoveCartModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                throw new NotFoundException("Користувач не знайдений");
            }
            var product = await _context.products.FindAsync(model.ProductId);
            if (product == null)
            {
                throw new NotFoundException("Товар не знайдено");
            }

            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == model.UserId); 
            if (cart == null)
            {
                throw new NotFoundException("Кошик не знайдено");
            }

            var CartIttem = cart.Items.FirstOrDefault(ci => ci.ProductId == model.ProductId);
            if (CartIttem == null)
            {
                throw new NotFoundException("Товар не знайдено в кошику");
            }

            cart.Items.Remove(CartIttem);
            _context.cartItems.Remove(CartIttem); 

            await _context.SaveChangesAsync();

        }

        public async Task UpdateCartAsync(UpdateCartModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                throw new AuthorizationException("Користувач не знайдений");
            }
            var product = await _context.products.FindAsync(model.ProductId);
            if (product == null)
            {
                throw new NotFoundException("Товар не знайдено");
            }
            if (product.Quantity < model.Quantity)
            {
                throw new BadRequestException("Недостатньо товару на складі");
            }

            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == model.UserId); 
            if (cart == null)
            {
                throw new NotFoundException("Кошик не знайдено");
            }
            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == model.ProductId); 
            if (cartItem == null)
            {
               throw new NotFoundException("Товар не знайдено в кошику");
            }

            if (model.Quantity >= 0)
            {
                cartItem.Quantity = model.Quantity;
            }
        }
    }
}
