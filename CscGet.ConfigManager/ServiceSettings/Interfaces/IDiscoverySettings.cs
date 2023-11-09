using System;

namespace CscGet.ConfigManager.ServiceSettings.Interfaces
{
    public interface IDiscoverySettings
    {

        Uri BidManagementUri { get; }
        Uri PricingUri { get; }
        Uri CountryUri { get; }
        Uri LaborRatesUri { get; }
        Uri DabManagementUri { get; }
        Uri RestrictionsUri { get; }
        Uri PricePerformanceUri { get; }
        string SecretKey { get; }
        string SuperAdminJwtToken { get; }
    }
}
