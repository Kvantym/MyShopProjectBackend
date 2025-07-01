using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.DTO;
using MyShopProjectBackend.Models;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;

namespace MyShopProjectBackend.Servises
{
    public class CartServises : ICartServises
    {
        private readonly AppDbConection _context;

        public CartServises(AppDbConection context)
        {
            _context = context;
        }

        public async Task<(bool Success, string? ErrorMessage)> AddToCartAsync(AddToCartModel model)
        {
            var user = await _context.users.FindAsync(model.UserId);
            if (user == null)
            {
                return (false, "Користувача не знайдено");
            }
            var product = await _context.products.FindAsync(model.ProductId);
            if (product == null)
            {
                return (false, "Товар не знайдено");
            }
            if (product.Quantity < model.Quantity)
            {
                return (false,"Недостатньо товару на складі");
            }
            if (model.Quantity <= 0)
            {
                return (false, "Кількість повинна бути більше 0");
            }

            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == model.UserId); // Отримуємо кошик користувача і його товари
            if (cart == null)
            {
                cart = new Models.Cart
                {
                    UserId = model.UserId,
                    Items = new List<Models.CartItem>()
                };
                await _context.carts.AddAsync(cart); // Якщо кошик не існує, створюємо новий
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == model.ProductId); // Перевіряємо, чи товар вже є в кошику
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
                cartItem.Quantity += model.Quantity;  // Додаємо до наявної кількості
            }
            await _context.SaveChangesAsync(); // Зберігаємо зміни в базі даних

            return (true, null); // Повертаємо статус успіху
        }

        public async Task<(bool Success, string? ErrorMessage)> CheckoutAsync(int userId)
        {
            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return (false, "Користувач не знайдений");
            }

            var cart = await _context.carts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                return (false, "Кошик порожній або не знайдено");
            }

            // Перевірка наявності товарів
            foreach (var item in cart.Items)
            {
                if (item.Product == null)
                {
                    return (false, $"Товар з ID {item.ProductId} не знайдено");
                }

                if (item.Product.Quantity < item.Quantity)
                {
                    return (false, $"Недостатньо товару {item.Product.Name} на складі");
                }
            }

            // Створення замовлення
            var order = new Models.Order
            {
                BuyerId = userId,
                Status = ShopOrderStatus.Confirmed.ToString(),
                OrderItems = new List<Models.OrderItem>()
            };

            foreach (var item in cart.Items)
            {
                // Зменшення кількості товару на складі
                item.Product.Quantity -= item.Quantity;

                order.OrderItems.Add(new Models.OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                });
            }

            _context.orders.Add(order);

            // Видалення товарів з кошика
            _context.cartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage)> ClearCartAsync(int userId)
        {
            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return (false, "Користувач не знайдений");
            }
            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId); // Отримуємо кошик користувача і його товари
            if (cart == null)
            {
                return (false, "Кошик не знайдено");
            }
            _context.cartItems.RemoveRange(cart.Items); // Видаляємо всі товари з кошика
            await _context.SaveChangesAsync();

            return (true, null);
        }

        public async Task<(bool Success, string? ErrorMessage, CartDto? CartDto)> GetCartAsync(int userId)
        {
            var user = await _context.users.FindAsync(userId);// Отримуємо користувача за ID
                                                              // Якщо користувач не знайдений, повертаємо 404 Not Found
            if (user == null)
            {
                return (false, "Користувач не знайдений", null);
            }
            var cart = await _context.carts.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return (false, "Кошик не знайдено", null);
            }

            var carItems = await _context.cartItems
                .Include(ci => ci.Product) // Завантажуємо продукт для кожного товару в кошику
                .Where(ci => ci.CartId == cart.Id) // Фільтруємо товари по ID кошика
                .ToListAsync(); // Отримуємо список товарів в кошику

            var CartDto = new DTO.CartDto// DTO для повернення даних
            {
                UserId = cart.UserId,// ID користувача, якому належить кошик
                Id = cart.Id,// ID кошика

                Items = carItems.Select(c => new DTO.CartItemDto// DTO для кожного товару в кошику
                {
                    Id = c.Id,// ID товару в кошику
                    Quantity = c.Quantity,// Кількість товару в кошику
                    ProductId = c.ProductId,// ID продукту
                    ProductName = c.Product.Name,// Назва продукту
                    ProductPrice = c.Product.Price//    Ціна продукту
                }).ToList()
            };

            return (true, null, CartDto);
        }

        public async Task<(bool Success, string? ErrorMessage)> RemoveFromCartAsync(RemoveCartModel model)
        {
            var user = await _context.users.FindAsync(model.UserId);
            if (user == null)
            {
                return (false,"Користувач не знайдений");
            }
            var product = await _context.products.FindAsync(model.ProductId);
            if (product == null)
            {
                return (false, "Товар не знайдено");
            }

            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == model.UserId); // Отримуємо кошик користувача і його товари
            if (cart == null)
            {
                return (false, "Кошик не знайдено");
            }

            var CartIttem = cart.Items.FirstOrDefault(ci => ci.ProductId == model.ProductId); // Перевіряємо, чи товар є в кошику
            if (CartIttem == null)
            {
                return (false, "Товар не знайдено в кошику");
            }

            cart.Items.Remove(CartIttem); // Видаляємо товар з кошика
            _context.cartItems.Remove(CartIttem); // Видаляємо товар з таблиці товарів в кошику

            await _context.SaveChangesAsync(); // Зберігаємо зміни в базі даних

            return (true,null);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateCartAsync(UpdateCartModel model)
        {
            var user = await _context.users.FindAsync(model.UserId);
            if (user == null)
            {
                return (false, "Користувача не знайдено");
            }
            var product = await _context.products.FindAsync(model.ProductId);
            if (product == null)
            {
                return (false, "Товар не знайдено");
            }
            if (product.Quantity < model.Quantity)
            {
                return (false, "Недостатньо товару на складі");
            }

            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == model.UserId); // Отримуємо кошик користувача і його товари
            if (cart == null)
            {
                return (false, "Кошик не знайдено");
            }
            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == model.ProductId); // Перевіряємо, чи товар є в кошику
            if (cartItem == null)
            {
                return (false, "Товар не знайдено в кошику");
            }

            if (model.Quantity >= 0)
            {
                cartItem.Quantity = model.Quantity; // Оновлюємо кількість товару в кошику
            }

            await _context.SaveChangesAsync(); // Зберігаємо зміни в базі даних

            return (true, null);
        }
    }
}
