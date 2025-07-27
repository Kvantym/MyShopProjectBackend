namespace MyShopProjectBackend.Services.Exceptions
{
    public static class ExceptionExtensions
    {
        public static void ThrowExceptionIfNull<T>(this object obj, string messege) where T : Exception, new()
        {
            if (obj == null)
            {
                throw (T)Activator.CreateInstance(typeof(T), messege);
            }
        }
        public static void ThrowExceptionIfConditionTrue<T>(this bool condition, string messege) where T : Exception, new()
        {
            if (condition)
            {
                throw (T)Activator.CreateInstance(typeof(T), messege);
            }
        }
        public static void ThrowArgumentExceptionIfNull<T>(this object obj, string paramName) where T : ArgumentNullException, new() 
        {
            if (obj == null)
            {
                throw (T)Activator.CreateInstance(typeof(T), paramName);
            }
        }
        public static void ThrowArgumentExceptionIfNull<T>(this object obj, string paramName, string message) where T : ArgumentNullException, new() 
        {
            if (obj == null)
            {
                throw (T)Activator.CreateInstance(typeof(T), paramName, message);
            }
        }

    }
}
