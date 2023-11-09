using CscGet.ConfigManager.ServiceSettings.Models;

namespace CscGet.ConfigManager.ServiceSettings.Interfaces
{
    public interface ILoggerSettings
    {
        LoggerData LoggerSettings { get; }
    }
}
