namespace MyShopProjectBackend.Models
{
    public class ProductReview
    {
        public int Id { get; set; } // Ідентифікатор відгуку
        public int ProductId { get; set; } // Ідентифікатор продукту, до якого належить відгук
        public Product Product { get; set; } // Продукт, до якого належить відгук
        public int UserId { get; set; } // Ідентифікатор користувача, який залишив відгук
        public User User { get; set; } // Користувач, який залишив відгук
        public string ReviewText { get; set; } // Текст відгуку
        public int Rating { get; set; } // Рейтинг продукту (наприклад, від 1 до 5)
        public DateTime CreatedAt { get; set; } // Дата і час створення відгуку
        public ProductReview()
        {
            CreatedAt = DateTime.UtcNow; // Встановлюємо дату і час створення при створенні об'єкта
        }
    }
}
