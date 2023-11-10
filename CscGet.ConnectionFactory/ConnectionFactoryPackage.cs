using Microsoft.Extensions.DependencyInjection;

namespace CscGet.ConnectionFactory
{
    public class ConnectionFactoryPackage
    {
        public static IServiceCollection RegisterConnectionFactoryServices(IServiceCollection services)
        {
            services.AddScoped<ISqlConnectionProvider, CostingSqlConnectionProvider>();

            return services;
        }
    }
}