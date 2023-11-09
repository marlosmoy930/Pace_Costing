using System;
using Newtonsoft.Json;

namespace CscGet.ConfigManager.ServiceSettings.Models
{
    public class EndPoint
    {
        [JsonProperty("rootRouteIdentifier")]
        public string RootRouteIdentifier { get; set; }

        [JsonProperty("serviceName")]
        public string EndpointServiceName { get; set; }

        [JsonProperty("schema")]
        public string Schema { get; set; }

        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("port")]
        public string Port { get; set; }

        public Uri Uri => new Uri($"{Schema}://{Host}:{Port}");
    }
}