using MyShopProjectBackend.Domain.Entities;
using MyShopProjectBackend.Domain.Enums;
using MyShopProjectBackend.Domain.Request.Cart;
using MyShopProjectBackend.Domain.Responses;
using MyShopProjectBackend.Repositories.Interfaces;
using MyShopProjectBackend.Services.Exceptions;
using MyShopProjectBackend.Services.Interface;

namespace MyShopProjectBackend.Services.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserService _userService;
        private readonly IProductService _productService;

        public CartService( ICartRepository cartRepository,  IOrderRepository orderRepository, IUserService userService, IProductService productService)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _userService = userService;
            _productService = productService;
        }
        public async Task AddToCartAsync(AddToCartRequest request, string userId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var product = await _productService.GetProductOrThrowIdAsync(request.ProductId);

            var cart = await _cartRepository.GetCartByUserAsync(user);

            var cartItem = new Domain.Entities.CartItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Product = product,
                CartId = cart.Id,
            };

            await _cartRepository.AddCartItemAsync(cartItem);
        }

      public  async  Task CheckoutAsync(string userId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var cart = await GetCartOrThrowAsync(user);

            foreach (var item in cart.Items)
            {
                if (item.Product == null)
                {
                    throw new NotFoundException($"Товар з ID {item.ProductId} не знайдено");
                }
                if (item.Product.Quantity < item.Quantity)
                {
                    throw new NotFoundException($"Недостатньо товару {item.Product.Name} на складі");
                }
            }

            var order = new Order
            {
                BuyerId = userId,
                Status = ShopOrderStatus.Confirmed,
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
            await _orderRepository.CreateOrderAsync(order);

            await _cartRepository.ClearCartAsync(cart);
        }

        public async Task ClearCartAsync(string userId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var cart = await GetCartOrThrowAsync(user);

            await _cartRepository.ClearCartAsync(cart);
        }

        public async Task<CartResponse> GetCartAsync(string userId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var cart = await GetCartOrThrowAsync(user);

            var cartItems = await _cartRepository.GetCartItemsAsync(cart);
            if (cartItems == null || cartItems.Count == 0)
            {
               throw new NotFoundException("Cart is empty");
            }

            var cartResponse = new CartResponse
            {
                UserId = user.Id,
                Id = cart.Id,
                Items = cartItems.Select(ci => new CartItemResponse
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    ProductName = ci.Product.Name,
                    ProductPrice = ci.Product.Price,
                }).ToList(),
            };
            return cartResponse;
        }

        public async Task RemoveFromCartAsync(int productId, string userId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var cart = await GetCartOrThrowAsync(user);

            var product = await _productService.GetProductOrThrowIdAsync(productId);

            var cartItem = await _cartRepository.GetCartItemAsync(cart, productId);
            if (cartItem == null) 
            {
                throw new BadRequestException("cartItem not found");
            }

            await _cartRepository.RemoveCartItemAsync(cart, cartItem);
        }

        public async Task UpdateCartAsync(UpdateCartRequest request, string userId)
        {
            var user = await _userService.GetUserOrThrowAsyncId(userId);

            var cart = await GetCartOrThrowAsync(user);

            var product = await _productService.GetProductOrThrowIdAsync(request.ProductId);

            var cartItem = await _cartRepository.GetCartItemAsync(cart, request.ProductId);
            if (cartItem == null)
            {
                throw new BadRequestException("Cart item not found");
            }
            cartItem.Quantity = request.Quantity;
            await _cartRepository.UpdateCartItemAsync(cartItem);
        }
        public async Task<Cart> GetCartOrThrowAsync(ApplicationUser user)
        {
            var cart = await _cartRepository.GetCartByUserAsync(user);
            if (cart == null)
            {
                throw new NotFoundException("Cart not found");
            }
            return cart;
        }
    }
}
