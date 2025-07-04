namespace MyShopProjectBackend.Models
{
    public class FavoritProduct
    {
        public int Id { get; set; } // Ідентифікатор улюбленого продукту
        public int UserId { get; set; } // Ідентифікатор користувача, який додав продукт до улюблених
      //  public User User { get; set; } // Користувач, який додав продукт до улюблених
        public int ProductId { get; set; } // Ідентифікатор продукту, який додано до улюблених
        public Product Product { get; set; } // Продукт, який додано до улюблених
    }
}
