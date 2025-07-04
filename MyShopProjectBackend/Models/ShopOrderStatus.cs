namespace MyShopProjectBackend.Models
{


    public static  class ShopOrderStatus
    {
       public  const string Pending = "Очікується";    
        public const string Completed = "Виконано";  
        public const string Cancelled = "Скасовано";  
        public const string InProgress = "В процесі"; 
        public const string Refunded = "Повернено";   
        public const string Shipped = "Відправлено";    
        public const string Delivered = "Доставлено";  
        public const string Confirmed = "Підтверджено";   
    }
}
