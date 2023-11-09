using System;

namespace CscGet.ConfigManager.ServiceSettings.Interfaces
{
    public interface ICostingServiceSettings : IRabbitMqSettings, IRedisSettings
    {
        TimeSpan AllocationServiceTimeout { get; }
        string NotificationQueuePostFix { get; }
        bool IsDevelopmentEnvironment { get; }
        bool IsInflationEnabled { get; }

        TimeSpan HangfireCommandBatchMaxTimeout { get; }

        TimeSpan HangfireSlidingInvisibilityTimeout { get; }

        TimeSpan HangfireCommandTimeout { get; }

        TimeSpan HangfireTransactionTimeout { get; }

        int BulkOperationSagaStepTimeoutInSeconds { get; }
    }
}
