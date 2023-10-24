using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Csc.Get.Reporting.ExternalIntegration.Models;
using Dxc.Captn.Restrictions.Client.ApiClients.Interfaces;

namespace Csc.Get.Reporting.ExternalIntegration.Services
{
    public class RestrictionServiceClient : IRestrictionServiceClient
    {
        private readonly IRestrictionsApiClient _restrictionsApiClient;

        public RestrictionServiceClient(IRestrictionsApiClient restrictionsApiClient)
        {
            _restrictionsApiClient = restrictionsApiClient;
        }

        public async Task<IReadOnlyCollection<Restriction>> GetRestrictionsAsync(int bidId)
        {
            var result = await _restrictionsApiClient.GetVersionRestrictions(bidId);

            return result.Select(x => new Restriction(x.EntityId, x.IsGeneralRestricted, x.IsQuantityRestricted,
                x.IsElementRestricted, x.IsTermsRestricted, x.IsHidden)).ToList();
        }
    }
}