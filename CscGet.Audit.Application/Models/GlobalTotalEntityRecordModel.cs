using System;
using CscGet.Audit.Domain.Models.Enums;

namespace CscGet.Audit.Application.Models
{
    public class GlobalTotalEntityRecordModel
    {
        public int CostingVersionId { get; }
        public Guid EntityId { get; }
        public GroupType NewGroupType { get; }
        public GroupType OldGroupType { get; }
        public DateTime ModificationTime { get; }
        public string OldEntityName { get; }
        public string NewEntityName { get; }
        public Guid UserId { get; }
        public string Username { get; }

        public GlobalTotalEntityRecordModel(int costingVersionId, Guid entityId, GroupType newGroupType, GroupType oldGroupType, DateTime modificationTime, string oldEntityName, string newEntityName, Guid userId, string username)
        {
            CostingVersionId = costingVersionId;
            EntityId = entityId;
            NewGroupType = newGroupType;
            OldGroupType = oldGroupType;
            ModificationTime = modificationTime;
            OldEntityName = oldEntityName;
            NewEntityName = newEntityName;
            UserId = userId;
            Username = username;
        }
    }
}
