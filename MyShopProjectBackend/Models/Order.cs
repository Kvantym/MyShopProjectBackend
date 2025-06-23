namespace MyShopProjectBackend.Models
{
    public class Order
    {
        public int Id { get; set; } // Ід замовлення
        public DateTime  CreateAt { get; set; } = DateTime.Now; // Дата створення замовлення
        public int BuyerId { get; set; } // Ідентифікатор покупця (користувача, який зробив замовлення)
        public User Buyer { get; set; } // Покупець (користувач, який зробив замовлення)
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Колекція товарів в замовленні
        public ICollection<ShopOrder> ShopOrders { get; set; } = new List<ShopOrder>(); // Колекція магазинів, в яких були зроблені замовлення
    }
}
