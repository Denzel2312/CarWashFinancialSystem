using CarWashLoggingServer.Models;

namespace CarWashLoggingServer.Services
{
    public class LogService
    {
        private readonly string _logFile;

        public LogService()
        {
            // Логи будут в папке приложения
            _logFile = Path.Combine(Directory.GetCurrentDirectory(), "carwash_logs.txt");
        }

        // Добавляем новую запись в лог
        public void AddLog(string message, string level = "Информация", string user = "", string component = "")
        {
            var logEntry = new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = level,
                Message = message,
                User = user,
                Component = component
            };

            // Просто записываем в текстовый файл
            File.AppendAllText(_logFile,
                $"{logEntry.Timestamp:yyyy-MM-dd HH:mm:ss} | {logEntry.Level} | {logEntry.User} | {logEntry.Component} | {logEntry.Message}{Environment.NewLine}");
        }

        // Читаем логи из файла
        public List<LogEntry> GetLogs(DateTime? fromDate = null, DateTime? toDate = null, string level = null)
        {
            var logs = new List<LogEntry>();

            if (!File.Exists(_logFile))
                return logs;

            foreach (var line in File.ReadAllLines(_logFile))
            {
                try
                {
                    var parts = line.Split('|');
                    if (parts.Length >= 5)
                    {
                        var log = new LogEntry
                        {
                            Timestamp = DateTime.Parse(parts[0].Trim()),
                            Level = parts[1].Trim(),
                            User = parts[2].Trim(),
                            Component = parts[3].Trim(),
                            Message = parts[4].Trim()
                        };

                        // Фильтрация
                        if (fromDate.HasValue && log.Timestamp < fromDate.Value)
                            continue;
                        if (toDate.HasValue && log.Timestamp > toDate.Value)
                            continue;
                        if (!string.IsNullOrEmpty(level) && log.Level != level)
                            continue;

                        logs.Add(log);
                    }
                }
                catch
                {
                    // Пропускаем некорректные строки
                }
            }

            return logs.OrderByDescending(l => l.Timestamp).ToList();
        }

        // Получаем статистику
        public object GetStats()
        {
            var logs = GetLogs();
            var today = DateTime.Today;

            return new
            {
                TotalLogs = logs.Count,
                TodayLogs = logs.Count(l => l.Timestamp.Date == today),
                Errors = logs.Count(l => l.Level == "Ошибка"),
                Warnings = logs.Count(l => l.Level == "Предупреждение"),
                UniqueUsers = logs.Select(l => l.User).Distinct().Count()
            };
        }
    }
}