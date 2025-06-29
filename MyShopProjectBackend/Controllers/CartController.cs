using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShopProjectBackend.Db;
using MyShopProjectBackend.Models;

namespace MyShopProjectBackend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {

        private readonly AppDbConection _context;
        // GET: CartController
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        public CartController(AppDbConection conection)
        {
            _context = conection;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<IActionResult> GetCart(int userId)
        {
            var user = await _context.users.FindAsync(userId);// Отримуємо користувача за ID
                                                              // Якщо користувач не знайдений, повертаємо 404 Not Found
            if (user == null)
            {
                return NotFound("Користувач не знайдений");
            }
            var cart = await _context.carts.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound("Кошик не знайдено");
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

            return Ok(CartDto);
        }


        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(int userId, int productId, int quantity)
        {
            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Користувач не знайдений");
            }
            var product = await _context.products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Товар не знайдено");
            }
            if (product.Quantity < quantity)
            {
                return BadRequest("Недостатньо товару на складі");
            }

            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId); // Отримуємо кошик користувача і його товари
            if (cart == null)
            {
                cart = new Models.Cart
                {
                    UserId = userId,
                    Items = new List<Models.CartItem>()
                };
                await _context.carts.AddAsync(cart); // Якщо кошик не існує, створюємо новий
            }

            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId); // Перевіряємо, чи товар вже є в кошику
            if (cartItem == null)
            {
                cart.Items.Add(new Models.CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }
            else
            {
                cartItem.Quantity += quantity;  // Додаємо до наявної кількості
            }
            await _context.SaveChangesAsync(); // Зберігаємо зміни в базі даних

            return Ok("Товар успішно додано до кошика");
        }

        [HttpPost("UpdateCart")]
        public async Task<IActionResult> UpdateCart(int userId, int productId, int quantity)
        {
            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Користувач не знайдений");
            }
            var product = await _context.products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Товар не знайдено");
            }
            if (product.Quantity < quantity)
            {
                return BadRequest("Недостатньо товару на складі");
            }

            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId); // Отримуємо кошик користувача і його товари
            if (cart == null)
            {
                return NotFound("Кошик не знайдено");
            }
            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId); // Перевіряємо, чи товар є в кошику
            if (cartItem == null)
            {
                return NotFound("Товар не знайдено в кошику");
            }

            if (quantity >= 0)
            {
                cartItem.Quantity = quantity; // Оновлюємо кількість товару в кошику
            }

            await _context.SaveChangesAsync(); // Зберігаємо зміни в базі даних

            return Ok("Кошик успішно оновлено");
        }



        [HttpPost("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(int userId, int productId)
        {
            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Користувач не знайдений");
            }
            var product = await _context.products.FindAsync(productId);
            if (product == null)
            {
                return NotFound("Товар не знайдено");
            }

            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId); // Отримуємо кошик користувача і його товари
            if (cart == null)
            {
                return NotFound("Кошик не знайдено");
            }

            var CartIttem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId); // Перевіряємо, чи товар є в кошику
            if (CartIttem == null)
            {
                return NotFound("Товар не знайдено в кошику");
            }

            cart.Items.Remove(CartIttem); // Видаляємо товар з кошика
            _context.cartItems.Remove(CartIttem); // Видаляємо товар з таблиці товарів в кошику

            await _context.SaveChangesAsync(); // Зберігаємо зміни в базі даних

            return Ok("Товар успішно видалено з кошика");
        }

        [HttpPost("ClearCart")]

        public async Task<IActionResult> ClearCart(int userId)
        {
            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Користувач не знайдений");
            }
            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId); // Отримуємо кошик користувача і його товари
            if (cart == null)
            {
                return NotFound("Кошик не знайдено");
            }
            _context.cartItems.RemoveRange(cart.Items); // Видаляємо всі товари з кошика
            await _context.SaveChangesAsync();

            return Ok("Кошик успішно очищено");
        }

        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout(int userId)
        {
            var user = await _context.users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("Користувач не знайдений");
            }
            var cart = await _context.carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId); // Отримуємо кошик користувача і його товари
            if (cart == null || !cart.Items.Any())
            {
                return BadRequest("Кошик порожній або не знайдено");
            }
            // Перевіряємо наявність товарів в кошику
            foreach (var item in cart.Items)
            {
                var product = await _context.products.FindAsync(item.ProductId);
                if (product == null)
                {
                    return NotFound($"Товар з ID {item.ProductId} не знайдено");
                }
                if (product.Quantity < item.Quantity)
                {
                    return BadRequest($"Недостатньо товару {product.Name} на складі");
                }
                product.Quantity -= item.Quantity; // Зменшуємо кількість товару на складі
            }

            // Створюємо нове замовлення
            var order = new Models.Order
            {
                BuyerId = userId,
                Status = ShopOrderStatus.Confirmed.ToString(), // Встановлюємо статус замовлення
                OrderItems = new List<Models.OrderItem>()

            };
            _context.orders.Add(order); // Додаємо замовлення до бази даних


            foreach (var item in cart.Items)
            {
                if (item.Product.Quantity < item.Quantity)
                {
                    return BadRequest($"Недостатньо товару {item.Product.Name} на складі");
                }

                var orderItem = new Models.OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price * item.Quantity // Зберігаємо ціну товару на момент оформлення замовлення
                };
                item.Product.Quantity -= item.Quantity; // Зменшуємо кількість товару на складі
                order.OrderItems.Add(orderItem); // Додаємо товар до замовлення
            }
            _context.orders.Add(order); // Додаємо замовлення до бази даних
            cart.Items.Clear(); // Очищаємо кошик після оформлення замовлення
            _context.cartItems.RemoveRange(cart.Items); // Видаляємо всі товари з таблиці товарів в кошику

            await _context.SaveChangesAsync(); // Зберігаємо зміни в базі даних

            return Ok("Замовлення успішно оформлено");
        }
    }
}
