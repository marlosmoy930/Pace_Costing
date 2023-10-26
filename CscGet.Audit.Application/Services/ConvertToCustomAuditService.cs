using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CscGet.Audit.Application.Models;
using CscGet.Audit.Domain.Models;
using CscGet.Audit.Domain.Models.Enums;
using CscGet.Audit.Persistence.NoSql.Repositories;
using Dxc.Captn.Costing.Contracts.Operations.ConvertToCustom;

namespace CscGet.Audit.Application.Services
{
    public class ConvertToCustomAuditService : IConvertToCustomAuditService
    {
        private readonly IAuditRecordRepository<ConvertToCustomAuditRecord, string> _auditRecordRepository;
        private readonly ICommonAuditRecordService<ConvertToCustomAuditRecord, string> _auditRecordService;

        public ConvertToCustomAuditService(IAuditRecordRepository<ConvertToCustomAuditRecord, string> auditRecordRepository, ICommonAuditRecordService<ConvertToCustomAuditRecord, string> auditRecordService)
        {
            _auditRecordRepository = auditRecordRepository;
            _auditRecordService = auditRecordService;
        }

        public async Task CreateRecord(CreateConversionAuditRecordCommand @event, string userName)
        {
            foreach (var convertedItem in @event.ConvertedNodes)
            {
                GroupType groupType;
                if (String.IsNullOrEmpty(convertedItem.GroupTypeCode))
                    groupType = GroupType.None;
                else if (!Enum.TryParse(convertedItem.GroupTypeCode, true, out groupType))
                    throw new InvalidOperationException($"Received unknown groupType {convertedItem.GroupTypeCode}");

                var record = ConvertToCustomAuditRecordFactory.CreateRecord(convertedItem, groupType, @event.CostingVersionId,
                    @event.TimestampUtc, @event.UserId, userName);

                await _auditRecordRepository.AddOrUpdateAsync(record).ConfigureAwait(false);
            }
        }

        public Task RemoveRecordsAsync(Guid[] ids)
        {
            return _auditRecordService.RemoveRecordsAsync(ids);
        }

        public Task CopyRecordsAsync(int costingVersionId, IReadOnlyCollection<EntityRecordCopyModel> recordCopyModels)
        {
            return _auditRecordService.CopyRecordsAsync(costingVersionId, recordCopyModels);
        }
    }
}
