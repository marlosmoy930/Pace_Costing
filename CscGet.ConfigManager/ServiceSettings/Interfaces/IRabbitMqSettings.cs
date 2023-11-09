using System;

namespace CscGet.ConfigManager.ServiceSettings.Interfaces
{
    public interface IRabbitMqSettings
    {
        string RabbitMqHost { get; }
        string UserName { get; }
        string Password { get; }
        TimeSpan ReportCalculationTimeout { get; }
        TimeSpan DashboardSummaryCalculationTimeout { get; }
    }
}
