using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CscGet.CommandDelivery.Extensions;
using CscGet.ConfigManager.ServiceSettings.Interfaces;
using CscGet.MessageContracts.Api;
using CscGet.MessageContracts.CalculationService.Commands;
using Dxc.Captn.Infrastructure.Configuration;
using Dxc.Captn.Infrastructure.Configuration.LogManager;
using Dxc.Pace.Operations.Contracts.Costing.Common;
using MassTransit;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Serilog;

namespace CscGet.CommandDelivery.Calculation
{
    public class CalculationService : ICalculationService
    {
        private readonly IBus _bus;
        private readonly IRabbitMqSettings _settings;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ICorrelationLogManager _correlationLogManager;

        public CalculationService(IBus bus, IRabbitMqSettings settings, ICorrelationLogManager correlationLogManager)
        {
            _bus = bus;
            _settings = settings;
            _correlationLogManager = correlationLogManager;
            _connectionFactory = new ConnectionFactory
            {
                HostName = new Uri(settings.RabbitMqHost).Host,
                UserName = settings.UserName,
                Password = settings.Password
            };
        }

        public void CalculateCostGroups(int bidId, IEnumerable<Guid> costGroupIds)
        {
            if (!costGroupIds.Any())
            {
                Log.Warning("CommandDelivery. Trying to send empty cost group array");
                return;
            }

            //in case of 1000+ cgs Calculation Service consumes 10+ GB of memory
            //we split groups to smaller parts to prevent errors and add ability to scale horizontally

            var uniqueCostGroupIds = costGroupIds.Distinct().OrderBy(p => p).ToList();
            var chunks = SplitToChunks(uniqueCostGroupIds, 10);

            var chunkTrackingIds = chunks.Select(p => p.First().ToString()).ToArray();
            _bus.Publish(new CostGroupCalculationStartedCommand
            {
                CommoditiesOfTypeCount = uniqueCostGroupIds.Count(),
                TrackingIds = chunkTrackingIds
            }, _correlationLogManager);

            foreach (var chunk in chunks)
            {
                _bus.SendEvent(ApiQueueConstants.CalculationCostGroupCalculationQueue.ToQueueUri(_settings.RabbitMqHost),
                    new CalculateCostGroupCommand
                    {
                        BidId = bidId,
                        CostGroupIds = chunk.ToArray()
                    },
                    _correlationLogManager);
            }
        }

        public async Task CalculateCostGroupsAsync(int bidId, IEnumerable<Guid> costGroupIds)
        {
            if (!costGroupIds.Any())
            {
                Log.Warning("CommandDelivery. Trying to send empty cost group array");
                return;
            }

            //in case of 1000+ cgs Calculation Service consumes 10+ GB of memory
            //we split groups to smaller parts to prevent errors and add ability to scale horizontally

            var uniqueCostGroupIds = costGroupIds.Distinct().OrderBy(p => p).ToList();
            var chunks = SplitToChunks(uniqueCostGroupIds, 10);

            var chunkTrackingIds = chunks.Select(p => p.First().ToString()).ToArray();
            await _bus.Publish(new CostGroupCalculationStartedCommand
            {
                CommoditiesOfTypeCount = uniqueCostGroupIds.Count(),
                TrackingIds = chunkTrackingIds
            }, _correlationLogManager);

            var tasks = chunks
                .Select(chunk =>
                    _bus.SendEventAsync(
                        ApiQueueConstants.CalculationCostGroupCalculationQueue.ToQueueUri(_settings.RabbitMqHost),
                        new CalculateCostGroupCommand
                        {
                            BidId = bidId,
                            CostGroupIds = chunk.ToArray()
                        },
                        _correlationLogManager))
                 .ToArray();

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public IReadOnlyCollection<QueueStatus> GetQueueStatuses()
        {
            var queueNames = new[]
            {
                ApiQueueConstants.CalculationCostGroupCalculationQueue,
                ApiQueueConstants.CalculateAllocationsQueue,
                ApiQueueConstants.LaborQueue,
                ApiQueueConstants.LaborRatesQueue,
                ApiQueueConstants.HardwareQueue
            };

            var result = new List<QueueStatus>(queueNames.Length);

            using (var connection = _connectionFactory.CreateConnection())
            {
                using (var model = connection.CreateModel())
                {
                    foreach (var queueName in queueNames)
                    {
                        try
                        {
                            var queue = model.QueueDeclarePassive(queueName);
                            result.Add(new QueueStatus { MessagesCount = queue.MessageCount, QueueName = queueName });
                        }
                        catch (OperationInterruptedException)
                        {
                            result.Add(new QueueStatus { MessagesCount = 0, QueueName = queueName });
                        }
                    }
                }
            }

            return result;
        }

        private List<List<T>> SplitToChunks<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
