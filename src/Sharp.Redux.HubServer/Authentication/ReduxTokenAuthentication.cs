using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Sharp.Redux.HubServer.Authentication
{
    public class ReduxTokenAuthentication : AuthenticationHandler<ReduxTokenAuthenticationOptions>
    {
        public ReduxTokenAuthentication(IOptionsMonitor<ReduxTokenAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) :
            base(options, logger, encoder, clock)
        {}

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var tokenKey = Request.Headers.Keys.FirstOrDefault(h => h.Equals(Options.HeaderName));
            if (string.IsNullOrWhiteSpace(tokenKey))
            {
                return Task.FromResult(AuthenticateResult.Fail("No authorization is found in request header."));
            }
            string fullTokenId = Request.Headers[tokenKey].ToString();
            if (string.IsNullOrWhiteSpace(fullTokenId) || !fullTokenId.StartsWith($"{ReduxTokenAuthenticationOptions.AuthenticationScheme} ", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("No token is found in request header."));
            }
            string tokenId = fullTokenId.Substring(ReduxTokenAuthenticationOptions.AuthenticationScheme.Length + 1);
            var token = Options.TokenStore.Get(tokenId);
            if (token == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Token id is invalid."));
            }

            var claimsIdentity = new ClaimsIdentity();
            claimsIdentity.AddClaim(new Claim(ReduxClaim.TokenId, tokenId));
            claimsIdentity.AddClaim(new Claim(ReduxClaim.ProjectId, token.ProjectId.ToString()));
            if (token.IsRead)
            {
                claimsIdentity.AddClaim(new Claim(ReduxClaim.IsRead, token.IsRead.ToString()));
            }
            if (token.IsWrite)
            {
                claimsIdentity.AddClaim(new Claim(ReduxClaim.IsWrite, token.IsWrite.ToString()));
            }
            var principal = new ClaimsPrincipal(claimsIdentity);

            var ticket = new AuthenticationTicket(principal, ReduxTokenAuthenticationOptions.AuthenticationScheme);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
