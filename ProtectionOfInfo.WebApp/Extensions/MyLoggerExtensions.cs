using Microsoft.Extensions.Logging;
using System;

namespace ProtectionOfInfo.WebApp.Extensions
{
    /// <summary>
    /// Отображение сообщения об ошибках в логах.
    /// </summary>
    public static class MyLoggerExtensions
    {
        #region EventIdentifiers

        private static readonly EventId DatabaseSavingErrorId = new(70040001, "DatabaseSavingError");
        private static readonly EventId NotificationAddedId = new(70040002, "NotificationAdded");
        private static readonly EventId NotificationProcessedId = new(70040003, "NotificationProcessed");
        private static readonly EventId UserAuntificatedId = new(70040003, "UserAuntificated");

        #endregion

        private static readonly Action<ILogger, string, Exception?> NotificationProcessedExecute =
            LoggerMessage.Define<string>(LogLevel.Information, NotificationProcessedId, "Processing for notification started {message}");

        private static readonly Action<ILogger, string, Exception?> NotificationAddedExecute =
            LoggerMessage.Define<string>(LogLevel.Information, NotificationAddedId, "New notification created: {subject}");

        private static readonly Action<ILogger, string, Exception?> DatabaseSavingErrorExecute =
            LoggerMessage.Define<string>(LogLevel.Error, DatabaseSavingErrorId, "{entityName}");

        private static readonly Action<ILogger, string, Exception?> UserAuntificatedExecute =
            LoggerMessage.Define<string>(LogLevel.Information, UserAuntificatedId, "{entityName}");

        public static void NotificationProcessed(this ILogger source, string message) => NotificationProcessedExecute(source, message, null);
        public static void NotificationAdded(this ILogger source, string subject) => NotificationAddedExecute(source, subject, null);
        public static void DatabaseSavingError(this ILogger source, string entityName, Exception? exception = null) => DatabaseSavingErrorExecute(source, entityName, exception);
        public static void UserAuntificated(this ILogger source, string message) => UserAuntificatedExecute(source, message, null);
    }
}
