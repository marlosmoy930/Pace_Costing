using System;

namespace CscGet.ConfigManager.ServiceSettings.Interfaces
{
    public interface IStartupSettings
    {
        string ServiceName { get; }
        string ServiceSchema { get; }
        string ServiceAddress { get; }
        int ServicePort { get; }

        string ConsulHost { get; }
        string NotificationQueuePostFix { get; }
        bool IsSwaggerEnabled { get; }
        bool IsDevelopmentEnvironment { get; }
        bool IsInflationEnabled { get; }

        bool IsHealthCheckEnabled { get; }
        int IntervalInSeconds { get; }
        int TimeoutInSeconds { get; }
        int DeregisterCriticalServiceInMinutes { get; }
        TimeSpan CacheServiceAddressesInterval { get; }
        bool UseConsulServiceDiscovery { get; }
    }
}
