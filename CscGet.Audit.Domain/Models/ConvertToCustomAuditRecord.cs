using System;
using CscGet.Audit.Domain.Models.Enums;

namespace CscGet.Audit.Domain.Models
{
    public sealed class ConvertToCustomAuditRecord : AuditRecord<string>, ICopyable<ConvertToCustomAuditRecord>
    {
        public ConvertToCustomAuditRecord(Guid id, GroupType groupType, int costingVersionId, DateTime modificationDate, Guid userId, string modifiedBy, string templateEntityName, string reason) : base(id, groupType, costingVersionId, modificationDate, userId, modifiedBy, templateEntityName, null, reason)
        {
        }

        public override bool IsCurrentValueSameAsTemplate()
        {
            return string.Equals(TemplateValue, CurrentValue, StringComparison.Ordinal);
        }

        public ConvertToCustomAuditRecord Copy(Guid newId, int newCostingVersionId)
        {
            return new ConvertToCustomAuditRecord(newId, GroupType, newCostingVersionId, ModificationDate, UserId, ModifiedBy, TemplateValue, Reason);
        }
    }
}
