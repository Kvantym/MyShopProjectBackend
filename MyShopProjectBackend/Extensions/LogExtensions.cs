namespace MyShopProjectBackend.Extensions
{
    public static class LogExtensions
    {
        private static readonly string _logPath = "E:\\MyProjects\\Logs\\LogsProjectShop.txt";

        //public static void LogInFileAndConsoleInformation(this ILogger _logger, string message)
        //{
        //    var logTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    var logEntry = $"{message} - {logTime}";

        //    _logger.LogInformation(logEntry);
        //    using (StreamWriter logWriter = new StreamWriter(_logPath, true))
        //    {
        //        logWriter.WriteLine(logEntry);
        //    }
        //}
        
        //public static void LogInFileAndConsoleWarning(this ILogger _logger, string message)
        //{
        //    var logTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    var logEntry = $"{message} - {logTime}";

        //    _logger.LogWarning(logEntry);
        //    using (StreamWriter logWriter = new StreamWriter(_logPath, true))
        //    {
        //        logWriter.WriteLine(logEntry);
        //    }
        //}
        //public static void LogInFileAndConsoleError(this ILogger _logger, string message)
        //{
        //    var logTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    var logEntry = $"{message} - {logTime}";

        //    _logger.LogError(logEntry);
        //    using (StreamWriter logWriter = new StreamWriter(_logPath, true))
        //    {
        //        logWriter.WriteLine(logEntry);
        //    }
        //}
        public static void LogInFileAndConsoleInformation(this ILogger _logger, string message) => Logs(_logger, message, LogLevel.Information);
        public static void LogInFileAndConsoleWarning(this ILogger _logger, string message) => Logs(_logger, message, LogLevel.Warning);
        public static void LogInFileAndConsoleError(this ILogger _logger, string message) => Logs(_logger, message, LogLevel.Error);
        public static void Logs(this ILogger _logger, string message, LogLevel logLevel)
        {
            var logTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"{message} - {logTime}";
            using (StreamWriter streamWriter = new StreamWriter(_logPath, true))
                switch (logLevel)
                {
                    case LogLevel.Information:
                        _logger.LogInformation(logEntry);
                        streamWriter.WriteLine(logEntry);
                        break;
                    case LogLevel.Error:
                        _logger.LogError(logEntry);
                        streamWriter.WriteLine(logEntry);
                        break;
                    case LogLevel.Warning:
                        _logger.LogWarning(logEntry);
                        streamWriter.WriteLine(logEntry);
                        break;
                }
        }
    }
}
