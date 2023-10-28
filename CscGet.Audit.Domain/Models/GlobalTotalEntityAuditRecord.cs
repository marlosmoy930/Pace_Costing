using System;
using CscGet.Audit.Domain.Models.Enums;

namespace CscGet.Audit.Domain.Models
{
    public sealed class GlobalTotalEntityAuditRecord : AuditRecord<string>, ICopyable<GlobalTotalEntityAuditRecord>
    {
        public GlobalTotalEntityAuditRecord(Guid id, GroupType groupType, int costingVersionId, DateTime modificationDate, Guid userId, string modifiedBy, string templateEntityName, string currentEntityName) : base(id, groupType, costingVersionId, modificationDate, userId, modifiedBy, templateEntityName, currentEntityName, AuditExceptionReasons.EntityInvolvedInGtRenamed)
        {
        }

        public override bool IsCurrentValueSameAsTemplate()
        {
            return string.Equals(TemplateValue, CurrentValue, StringComparison.Ordinal);
        }

        public GlobalTotalEntityAuditRecord Copy(Guid newId, int newCostingVersionId)
        {
            return new GlobalTotalEntityAuditRecord(newId, GroupType, newCostingVersionId, ModificationDate, UserId, ModifiedBy, TemplateValue, CurrentValue);
        }
    }
}
