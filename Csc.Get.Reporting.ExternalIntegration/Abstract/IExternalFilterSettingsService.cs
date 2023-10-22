using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Csc.Get.Reporting.ExternalIntegration.Models;
using BidState = Csc.Get.Reporting.ExternalIntegration.Models.BidState;
// TODO : get rid from BidState alias after dashboard-settings service change route

namespace Csc.Get.Reporting.ExternalIntegration.Abstract
{
    public interface IExternalFilterSettingsService
    {
        Task<bool> IsFilterApplied(int costingVersionId);
        // TODO : get rid from BidState alias after dashboard-settings service change route
        Task<IReadOnlyCollection<FilterData>> GetAllFilterSettings(int costingVersionId, BidState bidState);

        [Obsolete("Consider this method to be refactored, to return the (property, value)")]
        Task<IReadOnlyCollection<ExcludedPath>> GetExcludedPaths(int costingVersionId);
        Task CopyFilterSettingsAsync(int sourceCostingVersionId, int targetCostingVersionId);
    }
}
