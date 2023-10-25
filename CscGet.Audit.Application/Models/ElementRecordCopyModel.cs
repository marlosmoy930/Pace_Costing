using System;

namespace CscGet.Audit.Application.Models
{
    public sealed class ElementRecordCopyModel : EntityRecordCopyModel
    {
        public ElementRecordCopyModel(Guid sourceRecordId, Guid targetRecordId, Guid targetCostGroupId): base(sourceRecordId, targetRecordId)
        {
            TargetCostGroupId = targetCostGroupId;
        }

        public Guid TargetCostGroupId { get; }
    }
}
