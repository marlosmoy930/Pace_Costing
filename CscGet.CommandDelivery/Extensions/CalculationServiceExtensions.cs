using System;
using System.Threading.Tasks;
using CscGet.MessageContracts.Api;
using Dxc.Captn.Infrastructure.Configuration;
using Dxc.Captn.Infrastructure.Configuration.LogManager;
using MassTransit;

namespace CscGet.CommandDelivery.Extensions
{
    public static class CalculationServiceExtensions
    {
        public static void SendEvent<TMessage>(this IBus bus, Uri queue, TMessage message, ICorrelationLogManager correlationLogManager) where TMessage : class
        {
            Task.Run(async () => await ConfigureAndSend(bus, queue, message, correlationLogManager)).GetAwaiter().GetResult();
        }

        public static async Task SendEventAsync<TMessage>(this IBus bus, Uri queue, TMessage message, ICorrelationLogManager correlationLogManager) where TMessage : class
        {
            await ConfigureAndSend(bus, queue, message, correlationLogManager).ConfigureAwait(false);
        }

        private static async Task ConfigureAndSend<TMessage>(this IBus bus, Uri queue, TMessage message, ICorrelationLogManager correlationLogManager) where TMessage : class
        {
            var endPoint = await bus.GetSendEndpoint(queue).ConfigureAwait(false);
            await endPoint.Send(message, x =>
            {
                x.Headers.Set("LogCorrelationId", correlationLogManager.CorrelationId);
                x.Headers.Set("JwtToken", correlationLogManager.JwtToken);
                x.SetAwaitAck(false);
            }).ConfigureAwait(false);
        }

        internal static Uri ToQueueUri(this string queueName, string rabbitMqHost)
        {
            return new Uri(new Uri(rabbitMqHost), queueName);
        }
    }
}