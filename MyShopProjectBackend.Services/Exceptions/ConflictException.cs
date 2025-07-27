namespace MyShopProjectBackend.Services.Exceptions
{
    public class ConflictException : Exception
    {
        public ConflictException(string messege) : base(messege)
        {
            
        }
        public ConflictException(string messege, Exception innerException) : base(messege, innerException)
        {
            
        }
    }
}
