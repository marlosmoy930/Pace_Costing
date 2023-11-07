using System;

namespace CscGet.ConfigManager.ServiceSettings.Interfaces
{
    public interface IAllocationSettings
    {
        string AllocationsMongoDbConnectionString { get; }
        TimeSpan AllocationServiceTimeout { get; }
    }
}
