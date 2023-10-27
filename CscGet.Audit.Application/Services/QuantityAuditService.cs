using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using CscGet.Audit.Application.Models;
using CscGet.Audit.Application.Services.External;
using CscGet.Audit.Domain.Models;
using CscGet.Audit.Domain.Models.Enums;
using CscGet.Audit.Persistence.NoSql.Repositories;
using Dxc.Captn.Costing.Contracts.Quantity;
using MoreLinq;

namespace CscGet.Audit.Application.Services
{
    public class QuantityAuditService : IQuantityAuditService
    {
        private readonly IAuditRecordRepository<QuantitiesAuditRecord, Quantity[]> _auditRecordRepository;
        private readonly ICommonAuditRecordService<QuantitiesAuditRecord, Quantity[]> _auditRecordService;
        private readonly IDataSourceTypeService _dataSourceTypeService;

        private static readonly IReadOnlyCollection<string> IgnoredGroupTypes = new[]
        {
            GroupType.GT.ToString(),
            GroupType.GD.ToString(),
        };

        public QuantityAuditService(
            IAuditRecordRepository<QuantitiesAuditRecord, Quantity[]> auditRecordRepository,
            ICommonAuditRecordService<QuantitiesAuditRecord, Quantity[]> auditRecordService,
            IDataSourceTypeService dataSourceTypeService)
        {
            _auditRecordRepository = auditRecordRepository;
            _auditRecordService = auditRecordService;
            _dataSourceTypeService = dataSourceTypeService;
        }

        public async Task CreateOrUpdateExistingRecord(QuantitiesChangedEvent @event)
        {
            var containerModels = await GetEligibleQuantities(@event.CostingVersionId, @event.ChangedEntities);
            var containerIds = containerModels.Select(x => x.EntityId).ToHashSet();
            var existingRecords = (await _auditRecordRepository.GetByIdsAsync(containerIds)).ToDictionary(x => x.Id);

            var auditRecordsToCreate = GetRecordToCreate(@event, containerModels, existingRecords);
            var auditRecordsToUpdate = GetRecordToUpdate(@event, containerModels, existingRecords);

            if (auditRecordsToCreate.Any())
                await _auditRecordRepository.AddAsync(auditRecordsToCreate.ToArray());

            if (auditRecordsToUpdate.Any())
                await _auditRecordRepository.UpdateAsync(auditRecordsToUpdate.ToArray());
        }

        private static List<QuantitiesAuditRecord> GetRecordToCreate(QuantitiesChangedEvent @event,
            IReadOnlyCollection<QuantityContainerModel> containerModels,
            Dictionary<Guid, QuantitiesAuditRecord> existingRecords)
        {
            var auditRecordsToCreate = new List<QuantitiesAuditRecord>();
            foreach (var containerModel in containerModels.Where(x => !existingRecords.ContainsKey(x.EntityId)))
            {
                var updatedQuantities = containerModel.QuantitiesAfterChange.Select(x =>
                    new Quantity(x.Value, x.Date.Month, x.Date.Year, x.Formula)).ToArray();

                var templateQuantities = containerModel.QuantitiesBeforeChange.Select(x =>
                    new Quantity(x.Value, x.Date.Month, x.Date.Year, x.Formula)).ToArray();
                if (!Enum.TryParse(containerModel.GroupTypeCode, out GroupType groupType))
                    throw new InvalidOperationException($"Received unknown groupType {containerModel.GroupTypeCode}");

                var newRecord = new QuantitiesAuditRecord(containerModel.EntityId, groupType,
                    @event.CostingVersionId, @event.EventDate, @event.UserId, @event.Username, templateQuantities,
                    updatedQuantities, AuditExceptionReasons.QuantityTabChanged);

                if (newRecord.IsCurrentValueSameAsTemplate())
                    continue;

                auditRecordsToCreate.Add(newRecord);
            }

            return auditRecordsToCreate;
        }

        private static List<QuantitiesAuditRecord> GetRecordToUpdate(QuantitiesChangedEvent @event,
            IReadOnlyCollection<QuantityContainerModel> containerModels,
            Dictionary<Guid, QuantitiesAuditRecord> existingRecords)
        {
            var auditRecordsToUpdate = new List<QuantitiesAuditRecord>();
            foreach (var containerModel in containerModels.Where(x => existingRecords.ContainsKey(x.EntityId)))
            {
                var existingRecord = existingRecords[containerModel.EntityId];
                var updatedQuantities = containerModel.QuantitiesAfterChange.Select(x =>
                    new Quantity(x.Value, x.Date.Month, x.Date.Year, x.Formula)).ToArray();

                existingRecord.UpdateCurrentValue(updatedQuantities, @event.EventDate, @event.UserId, @event.Username);

                if (existingRecord.IsCurrentValueSameAsTemplate())
                    continue;

                auditRecordsToUpdate.Add(existingRecord);
            }

            return auditRecordsToUpdate;
        }

        public Task RemoveRecordsAsync(Guid[] ids)
        {
            return _auditRecordService.RemoveRecordsAsync(ids);
        }

        public Task CopyRecordsAsync(int costingVersionId, IReadOnlyCollection<EntityRecordCopyModel> recordCopyModels)
        {
            return _auditRecordService.CopyRecordsAsync(costingVersionId, recordCopyModels);
        }

        private async Task<IReadOnlyCollection<QuantityContainerModel>> GetEligibleQuantities(
            int costingVersionId, IReadOnlyCollection<QuantityContainerModel> containerModels)
        {
            var changedQuantities = containerModels.Where(x => !IgnoredGroupTypes.Contains(x.GroupTypeCode)).ToList();
            if (!changedQuantities.Any())
                return new List<QuantityContainerModel>(0);

            var entityIds = changedQuantities.Select(x => x.EntityId).ToList();
            var eligibleIds = await _dataSourceTypeService.FilterOutEntityIdsWithSctDataSource(costingVersionId, entityIds);

            var eligibleQuantities = changedQuantities.Where(x => eligibleIds.Contains(x.EntityId));

            return eligibleQuantities.ToList();
        }
    }
}
