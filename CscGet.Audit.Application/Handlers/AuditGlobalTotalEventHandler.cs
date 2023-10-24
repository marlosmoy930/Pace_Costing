using System;
using System.Threading.Tasks;
using CscGet.Audit.Application.Models;
using CscGet.Audit.Application.Services;
using CscGet.Audit.Domain.Models.Enums;
using CscGet.Costing.Domain.Dispatcher.Events.GlobalTotals;
using CscGet.Costing.Domain.Dispatcher.Handlers;
using MassTransit;

namespace CscGet.Audit.Application.Handlers
{
    public class AuditGlobalTotalEventHandler : IContextEventHandlerAsync<GlobalTotalUpdated>, IConsumer<GlobalTotalDeleted>
    {
        private readonly IGlobalTotalEntityAuditService _globalTotalEntityAuditService;
        private readonly IUserProvider _userProvider;

        public AuditGlobalTotalEventHandler(IGlobalTotalEntityAuditService globalTotalEntityAuditService, IUserProvider userProvider)
        {
            _globalTotalEntityAuditService = globalTotalEntityAuditService;
            _userProvider = userProvider;
        }

        public async Task Handle(GlobalTotalUpdated @event)
        {
            if (!Enum.TryParse(@event.NewGroupTypeCode, true, out GroupType newGroupType))
                throw new InvalidOperationException($"Received unknown groupType {@event.NewGroupTypeCode}");

            if (!Enum.TryParse(@event.OldGroupTypeCode, true, out GroupType oldGroupType))
                throw new InvalidOperationException($"Received unknown groupType {@event.OldGroupTypeCode}");

            if (!ShouldHandleEvent(@event.OldNodeName, @event.NewNodeName, @event.OldNodeDataSourceType, @event.ChangeReason, oldGroupType, newGroupType))
                return;
            
            string userName = _userProvider.GetCurrentUserName();
            var model = new GlobalTotalEntityRecordModel(@event.BidId, @event.NodeId, newGroupType, oldGroupType, @event.Timestamp, @event.OldNodeName, @event.NewNodeName, @event.UserId, userName);
            await _globalTotalEntityAuditService.AddOrUpdateRecordAsync(model).ConfigureAwait(false);
        }

        public async Task Consume(ConsumeContext<GlobalTotalDeleted> context)
        {
            var @event = context.Message;

            if (!Enum.TryParse(@event.NewGroupTypeCode, true, out GroupType newGroupType))
                throw new InvalidOperationException($"Received unknown groupType {@event.NewGroupTypeCode}");

            if (!Enum.TryParse(@event.OldGroupTypeCode, true, out GroupType oldGroupType))
                throw new InvalidOperationException($"Received unknown groupType {@event.OldGroupTypeCode}");

            if (!ShouldHandleEvent(@event.OldNodeName, @event.NewNodeName, @event.OldNodeDataSourceType, @event.DeletionReason, oldGroupType, newGroupType))
                return;

            string userName = _userProvider.GetCurrentUserName();
            var model = new GlobalTotalEntityRecordModel(@event.BidId, @event.NodeId, newGroupType, oldGroupType, @event.Timestamp, @event.OldNodeName, @event.NewNodeName, @event.UserId, userName);
            await _globalTotalEntityAuditService.AddOrUpdateRecordAsync(model).ConfigureAwait(false);
        }

        private static bool ShouldHandleEvent(string oldName, string newName, string dataSourceTypeName, GlobalTotalChangeReason changeReason, GroupType oldGroupType, GroupType newGroupType)
        {
            if (changeReason != GlobalTotalChangeReason.EntityUpdated)
                return false;

            if (string.Equals(oldName, newName, StringComparison.Ordinal) && oldGroupType == newGroupType)
                return false;

            if (!Enum.TryParse(dataSourceTypeName, true, out DataSourceType dataSourceType) || dataSourceType != DataSourceType.SCT)
                return false;

            return true;
        }
    }
}
