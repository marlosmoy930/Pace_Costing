using System.Linq;
using System.Security.Claims;
using Dxc.Captn.Infrastructure.Configuration.LogManager;
using Dxc.Captn.Infrastructure.Settings.Jwt.Configuration;
using JWT;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;

namespace CscGet.Audit.Application.Jwt
{
    public class JwtTokenUserProvider : IUserProvider
    {
        private readonly ICorrelationLogManager _correlationLogManager;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger _logger = Log.ForContext<JwtTokenUserProvider>();

        public JwtTokenUserProvider(ICorrelationLogManager correlationLogManager, IOptions<JwtSettings> jwtSettings)
        {
            _correlationLogManager = correlationLogManager;
            _jwtSettings = jwtSettings.Value;
        }

        public string GetCurrentUserName()
        {
            var jwtToken = _correlationLogManager.JwtToken;
            if (string.IsNullOrWhiteSpace(jwtToken))
                _logger.Warning("jwt token is empty");

            var decodedJwt =  ValidateAndDecodeJwt(jwtToken, _jwtSettings.JwtSecretKey);
            var payload = JsonConvert.DeserializeObject<JwtPayload>(decodedJwt);

            var givenName = payload.JwtClaims.SingleOrDefault(c => c.Key == ClaimTypes.GivenName)?.Value;
            var surName = payload.JwtClaims.SingleOrDefault(c => c.Key == ClaimTypes.Surname)?.Value;

            string userName = string.Empty;
            if (string.IsNullOrWhiteSpace(givenName) && string.IsNullOrWhiteSpace(surName))
                _logger.Warning("Given name and surname both are empty");
            else
                userName = $"{givenName} {surName}";

            return userName.Trim();
        }

        private string ValidateAndDecodeJwt(string jwtToken, string secret)
        {
            string json = CreateJwtDecoder().Decode(jwtToken, secret, true);
            return json;
        }

        private IJwtDecoder CreateJwtDecoder()
        {
            IJsonSerializer serializer = new JwtJsonSerializer();
            IDateTimeProvider provider = new UtcDateTimeProvider();
            IJwtValidator validator = new JwtValidator(serializer, provider);
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            return new JwtDecoder(serializer, validator, urlEncoder);
        }
    }
}
