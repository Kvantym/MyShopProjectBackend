using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopProjectBackend.Extensions;
using MyShopProjectBackend.Models.Cart;
using MyShopProjectBackend.Servises.Interface;

namespace MyShopProjectBackend.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {

        private readonly ICartServises _cartServises;


        public CartController(ICartServises cartServises)
        {
            _cartServises = cartServises;
        }
        [HttpGet("status")]
        public IActionResult Get() => Ok("API працює");//готово

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCart()//готово
        {
            var result = await _cartServises.GetCartAsync(User.GetUserId());
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody]AddToCartModel model)//готово
        {
            await _cartServises.AddToCartAsync(model, User.GetUserId());
            return Ok(new { message = "Товар успішно додано до кошика" });
        }
        [Authorize]
        [HttpPost("update-cart")]
        public async Task<IActionResult> UpdateCart([FromBody]UpdateCartModel model)//готово
        {
            await _cartServises.UpdateCartAsync(model, User.GetUserId());
            return Ok(new { message = "Кошик успішно оновлено" });
        }

        [Authorize]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromCart(int productId)//готово
        {
            await _cartServises.RemoveFromCartAsync(productId, User.GetUserId());
            return Ok(new { message = "Товар успішно видалено з кошика" });
        }

        [Authorize]
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()//готово
        {
            await _cartServises.ClearCartAsync(User.GetUserId());
            return Ok(new { message = "Кошик успішно очищено" });
        }

        [Authorize]
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()//готово
        {
            await _cartServises.CheckoutAsync(User.GetUserId());
            return Ok(new { message = "Замовлення успішно оформлено" });
        }
    }
}
