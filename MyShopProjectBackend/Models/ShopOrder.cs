namespace MyShopProjectBackend.Models
{
    public class ShopOrder
    {
        public int Id { get; set; } // Ідентифікатор замовлення в магазині
      
        public int ShopId { get; set; } // Ідентифікатор магазину, в якому було зроблено замовлення
        public Shop Shop { get; set; } // Магазин, в якому було зроблено замовлення
        public int OrderId { get; set; } // Ідентифікатор замовлення
        public Order Order { get; set; } // Замовлення, яке було зроблено в магазині
        public ShopOrderStatus Status { get; set; } // Статус замовлення в магазині (наприклад, "Очікується", "Виконано", "Скасовано" тощо)

    }
}
