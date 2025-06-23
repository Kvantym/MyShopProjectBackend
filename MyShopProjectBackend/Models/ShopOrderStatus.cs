namespace MyShopProjectBackend.Models
{
    public enum ShopOrderStatus
    {
        Pending,    // Очікується
        Completed,  // Виконано
        Cancelled,  // Скасовано
        InProgress, // В процесі
        Refunded,   // Повернено
        Shipped,    // Відправлено
        Delivered,  // Доставлено
        Confirmed   // Підтверджено
    }
}
