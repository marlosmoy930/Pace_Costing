using System.Collections.Generic;
using Newtonsoft.Json;

namespace CscGet.Audit.Application.Jwt
{
    internal sealed class JwtPayload
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("claims")]
        public List<JwtClaim> JwtClaims { get; set; }
    }
}
