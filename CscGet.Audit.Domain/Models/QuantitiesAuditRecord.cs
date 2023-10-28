using System;
using System.Linq;
using CscGet.Audit.Domain.Models.Enums;

namespace CscGet.Audit.Domain.Models
{
    public class QuantitiesAuditRecord : AuditRecord<Quantity[]>, ICopyable<QuantitiesAuditRecord>
    {
        public QuantitiesAuditRecord(Guid id, GroupType groupType, int costingVersionId, DateTime modificationDate, Guid userId, string modifiedBy, Quantity[] templateValue, Quantity[] currentValue, string reason) : base(id, groupType, costingVersionId, modificationDate, userId, modifiedBy, templateValue, currentValue, reason)
        {
        }

        public override bool IsCurrentValueSameAsTemplate()
        {
            return CurrentValue.SequenceEqual(TemplateValue);
        }

        public QuantitiesAuditRecord Copy(Guid newId, int newCostingVersionId)
        {
            return new QuantitiesAuditRecord(newId, GroupType, newCostingVersionId, ModificationDate, UserId, ModifiedBy, TemplateValue, CurrentValue, Reason);
        }
    }
}
