namespace MyShopProjectBackend.ViewModels
{
    public class UpdateProductModel
    {
        public int OwnerId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }

        // 🖼 Зображення у форматі байтів
        public byte[]? ImageData { get; set; }

        // MIME-тип (наприклад: image/jpeg, image/png)
        public string? ImageMimeType { get; set; }
    }
}
