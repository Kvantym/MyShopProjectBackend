namespace MyShopProjectBackend.Models
{
    public class OrderItem
    {
        public int Id { get; set; } // Ідентифікатор товару в замовленні
        public int OrderId { get; set; } // Ідентифікатор замовлення, до якого належить товар
        public Order Order { get; set; } // Замовлення, до якого належить товар
        public int ProductId { get; set; } // Ідентифікатор продукту, який замовлено
        public Product Product { get; set; } // Продукт, який замовлено
        public int Quantity { get; set; } // Кількість продукту в замовленні
        public decimal UnitPrice { get; set; } // Ціна за одиницю продукту на момент замовлення

    }
}
