namespace MyShopProjectBackend.Models
{
    public class Product
    {
        public int Id { get; set; } 
        public string Name { get; set; } = string.Empty; // Назва продукту
        public string Description { get; set; } // Опис продукту
        public string Category { get; set; } = string.Empty;    // Категорія продукту
        public decimal Price { get; set; } // Ціна продукту
        public decimal PriceTotal { get; set; } // Загальна ціна продукту (можливо, з урахуванням кількості)
        public int ShopId { get; set; } // Ідентифікатор магазину, до якого належить продукт
        public Shop Shop { get; set; } // Магазин, до якого належить продукт
        public int Quantity { get; set; } // Кількість продукту на складі
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Колекція замовлень, в яких присутній цей продукт
        public ICollection<ProductReview> productReviews  { get; set; } = new List<ProductReview>(); // Колекція улюблених продуктів, в яких присутній цей продукт
    }
}
