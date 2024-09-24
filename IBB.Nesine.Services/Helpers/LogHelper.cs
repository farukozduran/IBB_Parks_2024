using NLog;
using System;

namespace IBB.Nesine.Services.Helpers
{
    public static class LogHelper
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public static void LogInfo(string message)
        {
            logger.Info(message);
        }

        public static void LogError(string message, Exception ex = null)
        {
            if (ex == null)
            {
                logger.Error(message);
            }
            else
            {
                logger.Error(ex, message);
            }
        }
    }
}
