using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CscGet.Audit.Application.Factories;
using CscGet.Audit.Application.Models;
using CscGet.Audit.Domain.Models;
using CscGet.Audit.Persistence.NoSql.Repositories;
using CscGet.Costing.Domain.Dispatcher.Events.LaborRates;

namespace CscGet.Audit.Application.Services
{
    public class PyramidMixAuditService : IPyramidMixAuditService
    {
        private readonly IPyramidMixAuditRecordRepository _auditRecordRepository;
        private readonly IPyramidMixAuditRecordFactory _pyramidMixAuditRecordFactory;

        public PyramidMixAuditService(IPyramidMixAuditRecordRepository auditRecordRepository, IPyramidMixAuditRecordFactory pyramidMixAuditRecordFactory)
        {
            _auditRecordRepository = auditRecordRepository;
            _pyramidMixAuditRecordFactory = pyramidMixAuditRecordFactory;
        }

        public async Task CreateOrUpdateRecordsAsync(LaborRatePyramidMixValuesChangedEvent @event)
        {
            var existingRecords = await _auditRecordRepository.GetByCostGroupIdAsync(@event.CostGroupId).ConfigureAwait(false);
            var existingRecordsDictionary = existingRecords.ToDictionary(x => x.Id);
            var recordsFromEvent = _pyramidMixAuditRecordFactory.CreateAuditRecords(@event);
            
            var newRecords = new List<PyramidMixAuditRecord>();
            var updatedRecords = new List<PyramidMixAuditRecord>();
            var operationTasks = new List<Task>();

            foreach (var pyramidMixAuditRecord in recordsFromEvent)
            {
                PyramidMixAuditRecord existingRecord;
                if (existingRecordsDictionary.TryGetValue(pyramidMixAuditRecord.Id, out existingRecord))
                {
                    existingRecord.UpdateCurrentValue(pyramidMixAuditRecord.CurrentValue, pyramidMixAuditRecord.ModificationDate, pyramidMixAuditRecord.UserId, pyramidMixAuditRecord.ModifiedBy);
                    updatedRecords.Add(existingRecord);
                }
                else
                    newRecords.Add(pyramidMixAuditRecord);
            }

            if (updatedRecords.Any())
                operationTasks.Add(_auditRecordRepository.UpdateAsync(updatedRecords.ToArray()));
            
            if (newRecords.Any())
                operationTasks.Add(_auditRecordRepository.AddAsync(newRecords.ToArray()));

            await Task.WhenAll(operationTasks).ConfigureAwait(false);
        }

        public Task RemoveRecordsAsync(Guid[] ids)
        {
            return _auditRecordRepository.RemoveAsync(ids);
        }

        public Task RemoveRecordsForCostGroupsAsync(Guid[] costGroupIds)
        {
            return _auditRecordRepository.RemoveByCostGroupIdsAsync(costGroupIds);
        }

        public async Task CopyRecordsAsync(int targetCostingVersionId, IReadOnlyCollection<ElementRecordCopyModel> recordCopyModels)
        {
            var recordModelsDictionary = recordCopyModels.ToDictionary(x => x.SourceId);
            var sourceRecords = await _auditRecordRepository.GetByIdsAsync(recordModelsDictionary.Keys).ConfigureAwait(false);
            if (sourceRecords.Count == 0)
                return;

            var copiedRecords = sourceRecords.Select(x =>
            {
                var copyModel = recordModelsDictionary[x.Id];
                return x.Copy(copyModel.TargetId, copyModel.TargetCostGroupId, targetCostingVersionId);
            }).ToArray();
            await _auditRecordRepository.AddAsync(copiedRecords).ConfigureAwait(false);
        }
    }
}
