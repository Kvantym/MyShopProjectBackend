using MyShopProjectBackend.Servises.Interface;

namespace MyShopProjectBackend
{
    public class Helper
    {
        public IAccountService AccountService { get; }
        public ICartService CartServises { get; }
        public IFavoriteService FavoriteServises { get; }
        public IOrderService OrderServises { get; }
        public IProductService ProductServises { get; }
        public IReviewService ReviewServise { get; }
        public IShopService ShopServise { get; }
        public IUserService UserServise { get; }


        public Helper(IAccountService accountService, ICartService cartServises, IFavoriteService favoriteServises, IOrderService orderServises, IProductService productServises, IReviewService reviewServise, IShopService shopServise, IUserService userServise)
        {
            AccountService = accountService;
            CartServises = cartServises;
            FavoriteServises = favoriteServises;
            OrderServises = orderServises;
            ProductServises = productServises;
            ReviewServise = reviewServise;
            ShopServise = shopServise;
            UserServise = userServise;
        }

    }
}
