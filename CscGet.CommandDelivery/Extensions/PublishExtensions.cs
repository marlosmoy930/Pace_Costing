using System.Threading.Tasks;
using Dxc.Captn.Infrastructure.Configuration.LogManager;
using MassTransit;

namespace CscGet.CommandDelivery.Extensions
{
    public static class PublishExtensions
    {
        public static Task Publish<T>(this IBus bus, T @event, ICorrelationLogManager correlationLogManager)
        {
            return bus.Publish(@event, x =>
            {
                x.Headers.Set("LogCorrelationId", correlationLogManager.CorrelationId);
                x.Headers.Set("JwtToken", correlationLogManager.JwtToken);
            });
        }
    }
}
