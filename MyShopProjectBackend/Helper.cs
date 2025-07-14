using MyShopProjectBackend.Servises.Interface;

namespace MyShopProjectBackend
{
    public class Helper
    {
        public IAccountService AccountService { get; }
        public ICartServises CartServises { get; }
        public IFavoriteServises FavoriteServises { get; }
        public IOrderServises OrderServises { get; }
        public IProductServises ProductServises { get; }
        public IReviewServise ReviewServise { get; }
        public IShopServise ShopServise { get; }
        public IUserServise UserServise { get; }


        public Helper(IAccountService accountService, ICartServises cartServises, IFavoriteServises favoriteServises, IOrderServises orderServises, IProductServises productServises, IReviewServise reviewServise, IShopServise shopServise, IUserServise userServise)
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
