using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Csc.Get.Reporting.ExternalIntegration.Abstract;
using Csc.Get.Reporting.ExternalIntegration.Models;
using Dxc.Captn.DashboardSettings.Client.ApiClients;
using Dxc.Captn.DashboardSettings.Client.Contracts.CostingFilters;
using BidState = Csc.Get.Reporting.ExternalIntegration.Models.BidState;
using DashboardSettingsBidState = Dxc.Captn.DashboardSettings.Client.Contracts.CostingFilters.BidState;
// TODO : get rid from BidState alias after dashboard-settings service change route

namespace Csc.Get.Reporting.ExternalIntegration.Services
{
    public class ExternalFilterSettingsService : IExternalFilterSettingsService
    {
        private readonly ICostingFiltersApiClient _costingFiltersApiClient;

        public ExternalFilterSettingsService(ICostingFiltersApiClient costingFiltersApiClient)
        {
            _costingFiltersApiClient = costingFiltersApiClient ??
                                       throw new ArgumentNullException(nameof(costingFiltersApiClient));
        }

        public Task<bool> IsFilterApplied(int costingVersionId)
        {
            return _costingFiltersApiClient.IsFilterApplied(costingVersionId);
        }

        public async Task<IReadOnlyCollection<FilterData>> GetAllFilterSettings(int costingVersionId, BidState bidState)
        {
            var propertyModels = await
                _costingFiltersApiClient.GetAllFilterSettings(costingVersionId, (DashboardSettingsBidState)bidState);

            var filterData = propertyModels.Select(MapToFilterData).ToList();
            return filterData;
        }

        public async Task<IReadOnlyCollection<ExcludedPath>> GetExcludedPaths(int costingVersionId)
        {
            var propertyModels = await _costingFiltersApiClient.GetUncheckedFilterSettings(costingVersionId);
            var filterData = propertyModels.SelectMany(p => p.FilteredOutValues.Select(v =>
                new ExcludedPath
                {
                    Path = string.Join(".", p.Name, v)
                })).ToList();

            return filterData;
        }

        public Task CopyFilterSettingsAsync(int sourceCostingVersionId, int targetCostingVersionId)
        {
            return _costingFiltersApiClient.CopyFilterSettings(sourceCostingVersionId, targetCostingVersionId);
        }

        private static FilterData MapToFilterData(PropertyModel propertyModel)
        {
            return new FilterData()
            {
                Path = propertyModel.Path,
                Name = propertyModel.Name,
                IsSelected = propertyModel.IsSelected,
                Level = propertyModel.Level,
                AllowedInAllocations = propertyModel.AllowedInAllocations
            };
        }
    }
}
