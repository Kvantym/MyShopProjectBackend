using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Entities;
using MyShopProjectBackend.Exceptions;
using MyShopProjectBackend.Models.Cart;
using MyShopProjectBackend.Servises.Interface;
using NLog;
using ILogger = NLog.ILogger;


namespace MyShopProjectBackend.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly AppDbConection _context;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IUserService _userService;
        private readonly IProductService _productService;


        public CartService(AppDbConection context, IUserService userService, IProductService productService)
        {
            _context = context;
            _userService = userService;
            _productService = productService;
        }

        public async Task AddToCartAsync(AddToCartModel model, string userId)//готово
        {
            _logger.Info($"{nameof(AddToCartAsync)}: Виклик методу");
          
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var product = await _productService.GetProductOrThrowIdAsync(model.ProductId);
          
            _logger.Info($"{nameof(AddToCartAsync)}: Додавання товару {product.Name} (к-ть {model.Quantity}) користувачу {user.UserName}");
            if (model.Quantity <= 0)
            {
                _logger.Warn($"{nameof(AddToCartAsync)}: Кількість товару має бути більше нуля");
                throw new BadRequestException("Кількість товару має бути більше нуля");
            }

            if (product.Quantity < model.Quantity)
            {
                _logger.Warn($"{nameof(AddToCartAsync)}: Недостатньо товару на складі");
                throw new BadRequestException("Недостатньо товару на складі");
            }

            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };
                await _context.carts.AddAsync(cart);
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == model.ProductId);
            if (cartItem == null)
            {
                cart.Items.Add(new CartItem
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
            _logger.Info($"{nameof(AddToCartAsync)}: Товар {product.Name} додано в кошик користувача {user.UserName}");
        }

        public async Task CheckoutAsync(string userId)
        {
            _logger.Info($"{nameof(CheckoutAsync)}: Виклик методу");


            var user = await _userService.GetUserOrThrowAsyncId(userId);

            _logger.Info($"{nameof(CheckoutAsync)}: Оформлення замовлення для користувача {user.UserName}");
            var cart = await _context.carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.Items.Count == 0)
            {
                _logger.Error($"{nameof(CheckoutAsync)}: Кошик порожній або не знайдено");
                throw new NotFoundException("Кошик порожній або не знайдено");
            }

            foreach (var item in cart.Items)
            {
                if (item.Product == null)
                {
                    _logger.Error($"{nameof(CheckoutAsync)}: Товар з ID {item.ProductId} не знайдено");
                    throw new NotFoundException($"Товар з ID {item.ProductId} не знайдено");
                }

                if (item.Product.Quantity < item.Quantity)
                {
                    _logger.Error($"{nameof(CheckoutAsync)}: Недостатньо товару '{item.Product.Name}' на складі");
                    throw new NotFoundException($"Недостатньо товару {item.Product.Name} на складі");
                }
            }

            var order = new Order
            {
                BuyerId = userId,
                Status = ShopOrderStatus.Confirmed.ToString(),
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in cart.Items)
            {
                item.Product.Quantity -= item.Quantity;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                });
            }

            _context.orders.Add(order);
            _context.cartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(CheckoutAsync)}: Замовлення для користувача {user.UserName} оформлено успішно");
        }

        public async Task ClearCartAsync(string userId)
        {
            _logger.Info($"{nameof(ClearCartAsync)}: Виклик методу");

            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                _logger.Error($"{nameof(ClearCartAsync)}: Кошик не знайдено");
                throw new NotFoundException("Кошик не знайдено");
            }

            _context.cartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(ClearCartAsync)}: Кошик користувача {user.UserName} очищено");
        }

        public async Task<CartDto?> GetCartAsync(string userId)
        {
            _logger.Info($"{nameof(GetCartAsync)}: Виклик методу");

            var user = await _userService.GetUserOrThrowAsyncId(userId);
            _logger.Info($"{nameof(GetCartAsync)}: Отримання кошика користувача {user.UserName}");
            var cart = await _context.carts.SingleOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                _logger.Error($"{nameof(GetCartAsync)}: Кошик не знайдено");
                throw new NotFoundException("Кошик не знайдено");
            }

            var cartItems = await _context.cartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cart.Id)
                .ToListAsync();

            var cartDto = new CartDto
            {
                UserId = cart.UserId,
                Id = cart.Id,
                Items = cartItems.Select(c => new CartItemDto
                {
                    Id = c.Id,
                    Quantity = c.Quantity,
                    ProductId = c.ProductId,
                    ProductName = c.Product.Name,
                    ProductPrice = c.Product.Price
                }).ToList()
            };

            _logger.Info($"{nameof(GetCartAsync)}: Кошик користувача {user.UserName} отримано");

            return cartDto;
        }

        public async Task RemoveFromCartAsync(int productId, string userId)
        {
            _logger.Info($"{nameof(RemoveFromCartAsync)}: Виклик методу");

            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var product = await _productService.GetProductOrThrowIdAsync(productId);

            _logger.Info($"{nameof(RemoveFromCartAsync)}: Видалення товару {product.Name} з кошика користувача {user.UserName}");
            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                _logger.Error($"{nameof(RemoveFromCartAsync)}: Кошик не знайдено");
                throw new NotFoundException("Кошик не знайдено");
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == product.Id);
            if (cartItem == null)
            {
                _logger.Error($"{nameof(RemoveFromCartAsync)}: Товар не знайдено в кошику");
                throw new NotFoundException("Товар не знайдено в кошику");
            }

            cart.Items.Remove(cartItem);
            _context.cartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(RemoveFromCartAsync)}: Товар {product.Name} видалено з кошика користувача {user.UserName}");
        }

        public async Task UpdateCartAsync(UpdateCartModel model, string userId)
        {
            _logger.Info($"{nameof(UpdateCartAsync)}: Виклик методу");

            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var product = await _productService.GetProductOrThrowIdAsync(model.ProductId);

            _logger.Info($"{nameof(UpdateCartAsync)}: Оновлення товару {product.Name} в кошику користувача {user.UserName}");
            if (product.Quantity < model.Quantity)
            {
                _logger.Warn($"{nameof(UpdateCartAsync)}: Недостатньо товару на складі");
                throw new BadRequestException("Недостатньо товару на складі");
            }

            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                _logger.Error($"{nameof(UpdateCartAsync)}: Кошик не знайдено");
                throw new NotFoundException("Кошик не знайдено");
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == model.ProductId);
            if (cartItem == null)
            {
                _logger.Error($"{nameof(UpdateCartAsync)}: Товар не знайдено в кошику");
                throw new NotFoundException("Товар не знайдено в кошику");
            }

            if (model.Quantity >= 0)
            {
                cartItem.Quantity = model.Quantity;
            }

            await _context.SaveChangesAsync();

            _logger.Info($"{nameof(UpdateCartAsync)}: Оновлення товару {product.Name} в кошику користувача {user.UserName} завершено");
        }
    }
}
