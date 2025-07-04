using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Servises.Interface;
using MyShopProjectBackend.ViewModels;
using System.Security.Claims;

namespace MyShopProjectBackend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {

        private readonly ICartServises _cartServises;
        // GET: CartController
        [HttpGet]
        public ActionResult Index()
        {
           return Ok("Cart Controller is working");
        }
        public CartController(ICartServises cartServises)
        {
            _cartServises = cartServises;
        }
        [Authorize]
        [HttpGet("GetCart")]
        public async Task<IActionResult> GetCart()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Перевірка моделі на валідність
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Отримання ідентифікатора користувача з токена
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found");
            }
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID");
            }

            var result = await _cartServises.GetCartAsync(userId); // Виклик методу для отримання кошика користувача

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо отримання кошика не вдалось
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart(AddToCartModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Перевірка моделі на валідність
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Отримання ідентифікатора користувача з токена
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found");
            }
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID");
            }
            model.UserId = userId; // Присвоєння ID користувача до моделі

            var result = await _cartServises.AddToCartAsync(model); // Виклик методу для додавання товару в кошик

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо додавання не вдалось
            }
            return Ok(new { message = "Товар успішно додано до кошика" });
        }
        [Authorize]
        [HttpPost("UpdateCart")]
        public async Task<IActionResult> UpdateCart(UpdateCartModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Перевірка моделі на валідність
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Отримання ідентифікатора користувача з токена
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found");
            }
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID");
            }
            model.UserId = userId; // Присвоєння ID користувача до моделі
            var result = await _cartServises.UpdateCartAsync(model); // Виклик методу для оновлення товару в кошику

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо оновлення не вдалось
            }

            return Ok(new { message = "Кошик успішно оновлено" });
        }

        [Authorize]
        [HttpPost("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(RemoveCartModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Перевірка моделі на валідність
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Отримання ідентифікатора користувача з токена
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found");
            }
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID");
            }
            model.UserId = userId; // Присвоєння ID користувача до моделі

            var result = await _cartServises.RemoveFromCartAsync(model); // Виклик методу для видалення товару з кошика

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо видалення не вдалось
            }
            return Ok(new { message = "Товар успішно видалено з кошика" });
        }
        [Authorize]
        [HttpPost("ClearCart")]

        public async Task<IActionResult> ClearCart()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Перевірка моделі на валідність
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Отримання ідентифікатора користувача з токена
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found");
            }
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID");
            }

            var result = await _cartServises.ClearCartAsync(userId); // Виклик методу для очищення кошика

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо очищення не вдалось
            }
            return Ok(new { message = "Кошик успішно очищено" });

        }
        [Authorize]
        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Перевірка моделі на валідність
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Отримання ідентифікатора користувача з токена
            if (userIdClaim == null)
            {
                return Unauthorized("User ID claim not found");
            }
            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID");
            }

            var result = await _cartServises.CheckoutAsync(userId); // Виклик методу для очищення кошика

            if (!result.Success)
            {
                return BadRequest(result.ErrorMessage); // Повернення помилки, якщо очищення не вдалось
            }
            return Ok(new { message = "Замовлення успішно оформлено" });
        }
    }
}
