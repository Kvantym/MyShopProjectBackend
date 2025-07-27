namespace MyShopProjectBackend.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int ShopId { get; set; }
        public Shop Shop { get; set; }
        public int Quantity { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<ProductReview> productReviews { get; set; } = new List<ProductReview>();

        // 🖼 Зображення у форматі байтів
        public byte[]? ImageData { get; set; }

        // MIME-тип (наприклад: image/jpeg, image/png)
        public string? ImageMimeType { get; set; }
    }
}
