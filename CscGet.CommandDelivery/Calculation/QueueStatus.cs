using Newtonsoft.Json;

namespace CscGet.CommandDelivery.Calculation
{
    public class QueueStatus
    {
        [JsonProperty("MessagesCount")]
        public uint MessagesCount { get; set; }
        
        [JsonProperty("QueueName")]
        public string QueueName { get; set; }
    }
}