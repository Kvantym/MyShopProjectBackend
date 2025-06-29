namespace MyShopProjectBackend.Models
{


    public static  class ShopOrderStatus
    {
       public  const string Pending = "Очікується";    // Очікується
        public const string Completed = "Виконано";  // Виконано
        public const string Cancelled = "Скасовано";  // Скасовано
        public const string InProgress = "В процесі"; // В процесі
        public const string Refunded = "Повернено";   // Повернено
        public const string Shipped = "Відправлено";    // Відправлено
        public const string Delivered = "Доставлено";  // Доставлено
        public const string Confirmed = "Підтверджено";   // Підтверджено
    }
}
