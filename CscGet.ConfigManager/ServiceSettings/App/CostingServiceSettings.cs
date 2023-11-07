using System;
using CscGet.ConfigManager.ServiceSettings.Interfaces;
using Microsoft.Extensions.Configuration;

namespace CscGet.ConfigManager.ServiceSettings.App
{
    public class CostingServiceSettings : ICostingServiceSettings
    {
        private readonly IConfiguration _configuration;

        public CostingServiceSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TimeSpan AllocationServiceTimeout => TimeSpan.Parse(_configuration.GetSection("AllocationServiceTimeout").Value);
        public TimeSpan ReportCalculationTimeout => TimeSpan.Parse(_configuration.GetSection("ReportCalculationTimeout").Value);
        public TimeSpan DashboardSummaryCalculationTimeout => TimeSpan.Parse(_configuration.GetSection("DashboardSummaryCalculationTimeout").Value);
        public string NotificationQueuePostFix => _configuration.GetSection("NotificationQueuePostFix").Value;
        public bool IsDevelopmentEnvironment => Boolean.Parse(_configuration.GetSection("IsDevelopmentEnvironment").Value);
        public bool IsInflationEnabled => Boolean.Parse(_configuration.GetSection("IsInflationEnabled").Value);
        public string RabbitMqHost => _configuration.GetSection("RabbitMq:Host").Value;
        public string UserName => _configuration.GetSection("RabbitMq:UserName").Value;
        public string Password => _configuration.GetSection("RabbitMq:Password").Value;

        // Used by ICalculationsStatusManagerSettings
        public string RedisConnectionString => _configuration.GetSection("ConnectionStrings:RedisCache").Value;
        public TimeSpan CostingCalculationsInProgressTimeout => TimeSpan.Parse(_configuration.GetSection("CostingCalculationsInProgressTimeout").Value);

        public TimeSpan HangfireCommandBatchMaxTimeout => TimeSpan.Parse(_configuration.GetSection("HangfireCommandBatchMaxTimeout").Value);

        public TimeSpan HangfireSlidingInvisibilityTimeout => TimeSpan.Parse(_configuration.GetSection("HangfireSlidingInvisibilityTimeout").Value);

        public TimeSpan HangfireCommandTimeout => TimeSpan.Parse(_configuration.GetSection("HangfireCommandTimeout").Value);

        public TimeSpan HangfireTransactionTimeout => TimeSpan.Parse(_configuration.GetSection("HangfireTransactionTimeout").Value);

        public int BulkOperationSagaStepTimeoutInSeconds => int.Parse(_configuration.GetSection("BulkOperationSagaStepTimeoutInSeconds").Value);
    }
}
