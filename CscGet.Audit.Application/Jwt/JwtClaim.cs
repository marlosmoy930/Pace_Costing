using Newtonsoft.Json;

namespace CscGet.Audit.Application.Jwt
{
    internal sealed class JwtClaim
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
