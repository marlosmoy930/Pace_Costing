using System;
using CscGet.Audit.Domain.Models.Core;
using CscGet.Audit.Domain.Models.Enums;

namespace CscGet.Audit.Domain.Models
{
    public abstract class AuditRecord<TValue> : Entity<Guid> where TValue : class
    {
        public GroupType GroupType { get; }

        public int CostingVersionId { get; }

        public DateTime ModificationDate { get; private set; }

        public Guid UserId { get; private set; }

        public string ModifiedBy { get; private set; }

        public TValue TemplateValue { get; }

        public TValue CurrentValue { get; private set; }

        public string Reason { get; }

        protected AuditRecord(Guid id, GroupType groupType, int costingVersionId, DateTime modificationDate, Guid userId, string modifiedBy, TValue templateValue, TValue currentValue, string reason) : base(id)
        {
            if (costingVersionId <= 0)
                throw new ArgumentOutOfRangeException(nameof(costingVersionId));
            
            if (userId == default(Guid))
                throw new ArgumentOutOfRangeException(nameof(userId));

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentNullException(nameof(reason));

            GroupType = groupType;
            CostingVersionId = costingVersionId;
            ModificationDate = modificationDate;
            UserId = userId;
            ModifiedBy = modifiedBy ?? throw new ArgumentNullException(nameof(modifiedBy));
            TemplateValue = templateValue;
            CurrentValue = currentValue;
            Reason = reason;
        }

        public void UpdateCurrentValue(TValue currentValue, DateTime modificationDateTime, Guid userId, string modifiedBy)
        {
            if (userId == default(Guid))
                throw new ArgumentOutOfRangeException(nameof(userId));

            if (modificationDateTime < ModificationDate)
                throw new ArgumentOutOfRangeException("New modification date must be later then previous", nameof(modificationDateTime));

            if (string.IsNullOrWhiteSpace(modifiedBy))
                throw new ArgumentNullException(nameof(modifiedBy));

            ModificationDate = modificationDateTime;
            CurrentValue = currentValue;
            UserId = userId;
            ModifiedBy = modifiedBy;
        }

        public abstract bool IsCurrentValueSameAsTemplate();
    }
}
