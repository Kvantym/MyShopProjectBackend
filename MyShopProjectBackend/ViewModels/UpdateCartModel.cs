namespace MyShopProjectBackend.ViewModels
{
    public class UpdateCartModel
    {
        public int UserId { get; set; } // Ідентифікатор користувача, який додає товар до кошика
        public int ProductId { get; set; } // Ідентифікатор продукту, який додається до кошика
        public int Quantity { get; set; } // Кількість продукту, яка додається до кошика
    }
}
