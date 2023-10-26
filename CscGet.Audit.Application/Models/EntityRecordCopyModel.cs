using System;

namespace CscGet.Audit.Application.Models
{
    public class EntityRecordCopyModel
    {
        public EntityRecordCopyModel(Guid sourceId, Guid targetId)
        {
            SourceId = sourceId;
            TargetId = targetId;
        }

        public Guid SourceId { get; }

        public Guid TargetId { get; }
    }
}
