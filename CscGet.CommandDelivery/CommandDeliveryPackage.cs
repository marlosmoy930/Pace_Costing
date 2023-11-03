using CscGet.CommandDelivery.Calculation;
using CscGet.CommandDelivery.Notification;
using Microsoft.Extensions.DependencyInjection;

namespace CscGet.CommandDelivery
{
    public static class CommandDeliveryPackage
    {
        public static IServiceCollection RegisterCommandDeliveryServices(this IServiceCollection services)
        {
            services.RegisterDomainDependencies();

            return services;
        }

        private static IServiceCollection RegisterDomainDependencies(this IServiceCollection services)
        {
            services.AddTransient<ICalculationService, CalculationService>();
            services.AddTransient<INotificationService, NotificationService>();
            return services;
        }
    }
}