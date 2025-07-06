namespace MyShopProjectBackend.ViewModels.Add
{
    public class AddToCartModel
    {
       public string UserId { get; set; } 
        public int ProductId { get; set; } // Ідентифікатор продукту, який додається до кошика
        public int Quantity { get; set; } // Кількість продукту, яка додається до кошика
    }
}
