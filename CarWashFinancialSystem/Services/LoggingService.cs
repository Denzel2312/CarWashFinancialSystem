using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarWashFinancialSystem.Services
{
    public class LoggingService
    {
        private readonly string _logFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CarWash", "logs.txt");
        private readonly string _serverUrl = "http://localhost:5034/Home/AddLog";

        public LoggingService()
        {
            var logDir = Path.GetDirectoryName(_logFile);
            if (!Directory.Exists(logDir))
                Directory.CreateDirectory(logDir);
        }

        public void Log(string message, string level = "INFO", string user = null, string component = null)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"{timestamp} | {level} | {user ?? ""} | {component ?? ""} | {message}";

            // 1. Гарантированно пишем в файл
            File.AppendAllText(_logFile, logEntry + Environment.NewLine);

            // 2. Пытаемся отправить на сервер (в фоне)
            _ = TrySendToServer(message, level, user, component);
        }

        private async Task TrySendToServer(string message, string level, string user, string component)
        {
            try
            {
                using var client = new HttpClient();
                var logData = new
                {
                    Message = message,
                    Level = level,
                    User = user ?? "",
                    Component = component ?? ""
                };

                var json = JsonSerializer.Serialize(logData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await client.PostAsync(_serverUrl, content);
            }
            catch
            {
                // Игнорируем ошибки - у нас есть файловый лог
            }
        }

        // ПРОСТЫЕ СИНХРОННЫЕ МЕТОДЫ (без Async в названии)
        public void LogInfo(string message, string user = null, string component = null)
            => Log(message, "Информация", user, component);

        public void LogWarning(string message, string user = null, string component = null)
            => Log(message, "Предупреждение", user, component);

        public void LogError(string message, string user = null, string component = null)
            => Log(message, "Ошибка", user, component);

        public void LogUserAction(string action, string user)
            => Log($"Действие пользователя: {action}", "Информация", user, "Действие пользователя");

        public void LogSystem(string message)
            => Log(message, "Система", component: "Система");
    }
}