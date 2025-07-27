namespace MyShopProjectBackend.Services.Exceptions
{
    public class MethodNotAllowedException : Exception
    {
        public MethodNotAllowedException(string messege) : base(messege) 
        { 
        
        }
        public MethodNotAllowedException(string messege, Exception innerException) : base(messege, innerException)
        {

        }
    }
}
