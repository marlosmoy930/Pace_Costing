namespace CscGet.ConfigManager.ServiceSettings.Models
{
    public sealed class LoggerData
    {
        public string FluentdHost { get; set; }
        public int FluentdPort { get; set; }
        public string FluentdLogMinimumLevel { get; set; }
        public bool IsEnabledConsoleLog { get; set; }
        public bool IsEnabledFluentdLog { get; set; }
        public bool IsEnabledZipkinTracing { get; set; }
        public string LogMinimumLevel { get; set; }
    }
}
