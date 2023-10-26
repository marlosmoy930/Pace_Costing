using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CscGet.Audit.Application.Models;
using CscGet.Audit.Domain;
using CscGet.Audit.Domain.Models;
using CscGet.Audit.Persistence.NoSql.Repositories;

namespace CscGet.Audit.Application.Services
{
    public class CommonAuditRecordService<TAuditRecord, TValue> : ICommonAuditRecordService<TAuditRecord, TValue> where TAuditRecord : AuditRecord<TValue>, ICopyable<TAuditRecord> where TValue : class
    {
        private readonly IAuditRecordRepository<TAuditRecord, TValue> _auditRecordRepository;

        public CommonAuditRecordService(IAuditRecordRepository<TAuditRecord, TValue> auditRecordRepository)
        {
            _auditRecordRepository = auditRecordRepository;
        }

        public async Task CopyRecordsAsync(int targetCostingVersionId, IReadOnlyCollection<EntityRecordCopyModel> recordCopyModels)
        {
            Dictionary<Guid, EntityRecordCopyModel> recordModelsDictionary = recordCopyModels.ToDictionary(x => x.SourceId);
            var sourceRecords = await _auditRecordRepository.GetByIdsAsync(recordModelsDictionary.Keys).ConfigureAwait(false);
            if (sourceRecords.Count == 0)
                return;

            var copiedRecords = sourceRecords.Select(x => x.Copy(recordModelsDictionary[x.Id].TargetId, targetCostingVersionId)).ToArray();
            await _auditRecordRepository.AddAsync(copiedRecords).ConfigureAwait(false);
        }

        public Task RemoveRecordsAsync(Guid[] ids)
        {
            return _auditRecordRepository.RemoveAsync(ids);
        }
    }
}
