using System.Collections.Generic;
using System.Threading.Tasks;
using Csc.Get.Reporting.ExternalIntegration.Models;

namespace Csc.Get.Reporting.ExternalIntegration.Services
{
    public interface IRestrictionServiceClient
    {
        Task<IReadOnlyCollection<Restriction>> GetRestrictionsAsync(int bidId);
    }
}
