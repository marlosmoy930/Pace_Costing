using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CscGet.CommandDelivery.Calculation
{
    public interface ICalculationService
    {
        /// <summary>
        ///     Calculating cost group if exists
        ///     Removing all CalculatedData from MongoDB if group has been deleted
        /// </summary>
        /// <param name="bidId"></param>
        /// <param name="costGroupIds"></param>
        void CalculateCostGroups(int bidId, IEnumerable<Guid> costGroupIds);

        /// <summary>
        ///     Calculating cost group if exists
        ///     Removing all CalculatedData from MongoDB if group has been deleted
        /// </summary>
        /// <param name="bidId"></param>
        /// <param name="costGroupIds"></param>
        Task CalculateCostGroupsAsync(int bidId, IEnumerable<Guid> costGroupIds);

        IReadOnlyCollection<QueueStatus> GetQueueStatuses();
    }
}