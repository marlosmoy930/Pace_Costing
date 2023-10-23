using System;

namespace Csc.Get.Reporting.ExternalIntegration.Models
{
    public class Restriction
    {
        public Guid EntityId { get; }
        public bool IsGeneralRestricted { get; }
        public bool IsQuantityRestricted { get; }
        public bool IsElementRestricted { get; }
        public bool IsTermsRestricted { get; }
        public bool IsHidden { get; }

        public Restriction(Guid entityId, bool isGeneralRestricted, bool isQuantityRestricted, bool isElementRestricted,
            bool isTermsRestricted, bool isHidden)
        {
            EntityId = entityId;
            IsGeneralRestricted = isGeneralRestricted;
            IsQuantityRestricted = isQuantityRestricted;
            IsElementRestricted = isElementRestricted;
            IsTermsRestricted = isTermsRestricted;
            IsHidden = isHidden;
        }
    }
}
