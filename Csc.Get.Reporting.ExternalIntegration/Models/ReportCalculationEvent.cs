using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Csc.Get.Reporting.ExternalIntegration.Models
{
    [Serializable]
    public class ReportCalculationEvent : ReportBaseEvent
    {
        public ReportCalculationEvent(
            string id,
            ReportNotificationEvent eventType,
            int bidId,
            byte statusId, 
            string statusName,
            ReportTypeCodes reportType,
            Guid? userId) 
            : base(id, eventType)
        {
            BidId = bidId;
            StatusId = statusId;
            StatusName = statusName;
            ReportType = reportType;
            UserId = userId;
        }

        //TODO: Should be changed in contracts between FE and BE
        [JsonProperty("bidId")]
        public int BidId { get; }

        [JsonProperty("userId")]
        public Guid? UserId { get; }

        [JsonProperty("statusId")]
        public byte StatusId { get; }

        [JsonProperty("statusName")]
        public string StatusName { get; }

        [JsonConverter(typeof(StringEnumConverter), true)]
        [JsonProperty("reportType", DefaultValueHandling = DefaultValueHandling.Include)]
        public ReportTypeCodes ReportType { get; }
    }
}
