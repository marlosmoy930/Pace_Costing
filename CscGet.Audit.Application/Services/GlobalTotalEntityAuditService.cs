using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CscGet.Audit.Application.Models;
using CscGet.Audit.Domain.Models;
using CscGet.Audit.Persistence.NoSql.Repositories;

namespace CscGet.Audit.Application.Services
{
    public class GlobalTotalEntityAuditService : IGlobalTotalEntityAuditService
    {
        private readonly IAuditRecordRepository<GlobalTotalEntityAuditRecord, string> _auditRecordRepository;
        private readonly ICommonAuditRecordService<GlobalTotalEntityAuditRecord, string> _auditRecordService;

        public GlobalTotalEntityAuditService(IAuditRecordRepository<GlobalTotalEntityAuditRecord, string> auditRecordRepository, ICommonAuditRecordService<GlobalTotalEntityAuditRecord, string> auditRecordService)
        {
            _auditRecordRepository = auditRecordRepository;
            _auditRecordService = auditRecordService;
        }

        public async Task AddOrUpdateRecordAsync(GlobalTotalEntityRecordModel model)
        {
            GlobalTotalEntityAuditRecord existingRecord = await _auditRecordRepository.GetByIdAsync(model.EntityId).ConfigureAwait(false);
            if (existingRecord == null)
            {
                existingRecord = new GlobalTotalEntityAuditRecord(model.EntityId, model.NewGroupType, model.CostingVersionId, model.ModificationTime, model.UserId, model.Username, model.OldEntityName, model.NewEntityName);
            }
            else
            {
                existingRecord.UpdateCurrentValue(model.NewEntityName, model.ModificationTime, model.UserId, model.Username);
            }

            if (existingRecord.IsCurrentValueSameAsTemplate() && model.OldGroupType == model.NewGroupType)
            {
                return;
            }

            await _auditRecordRepository.AddOrUpdateAsync(existingRecord).ConfigureAwait(false);
        }

        public Task RemoveRecordsAsync(Guid[] ids)
        {
            return _auditRecordService.RemoveRecordsAsync(ids);
        }

        public Task CopyRecordsAsync(int targetCostingVersionId, IReadOnlyCollection<EntityRecordCopyModel> recordCopyModels)
        {
            return _auditRecordService.CopyRecordsAsync(targetCostingVersionId, recordCopyModels);
        }
    }
}
