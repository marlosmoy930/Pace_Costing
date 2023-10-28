using System;
using System.Linq;
using CscGet.Audit.Domain.Models.Enums;

namespace CscGet.Audit.Domain.Models
{
    public class PyramidMixAuditRecord : AuditRecord<PyramidMixValue[]>
    {
        public PyramidMixAuditRecord(Guid id, int costingVersionId, DateTime modificationDate, Guid userId, string modifiedBy, PyramidMixValue[] templateValue, PyramidMixValue[] currentValue, string reason, Guid costGroupId) : base(id, GroupType.LRT, costingVersionId, modificationDate, userId, modifiedBy, templateValue, currentValue, reason)
        {
            CostGroupId = costGroupId;
        }

        public Guid CostGroupId { get; }

        public override bool IsCurrentValueSameAsTemplate()
        {
            return CurrentValue.SequenceEqual(TemplateValue);
        }

        public PyramidMixAuditRecord Copy(Guid newId, Guid newCostGroupId, int newCostingVersionId)
        {
            return new PyramidMixAuditRecord(newId, newCostingVersionId, ModificationDate, UserId, ModifiedBy, TemplateValue, CurrentValue, Reason, newCostGroupId);
        }
    }
}
