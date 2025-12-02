namespace CarWashLoggingServer.Models
{
    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = "Информация";
        public string Message { get; set; } = "";
        public string User { get; set; } = "";
        public string Component { get; set; } = "";
    }
}