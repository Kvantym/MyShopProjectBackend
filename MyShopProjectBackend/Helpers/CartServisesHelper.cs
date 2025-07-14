using MyShopProjectBackend.Servises.Interface;

namespace MyShopProjectBackend.Helpers
{
    public class CartServisesHelper
    {
        public IUserServise UserServise { get; }
        public IProductServises ProductServises { get; }

        public CartServisesHelper(IUserServise userServise, IProductServises productServises)
        {
            UserServise = userServise;
            ProductServises = productServises;
        }
    }

}
