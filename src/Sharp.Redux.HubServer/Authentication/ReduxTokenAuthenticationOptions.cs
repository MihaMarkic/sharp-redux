using Microsoft.AspNetCore.Authentication;
using Sharp.Redux.HubServer.Services;
using System;

namespace Sharp.Redux.HubServer.Authentication
{
    public class ReduxTokenAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string AuthenticationScheme = "ReduxToken";
        public const string DefaultHeaderName = "X-Token";
        public string HeaderName { get; set; } = DefaultHeaderName;
        public ITokenStore TokenStore { get; set; }
        public override void Validate()
        {
            if (string.IsNullOrEmpty(HeaderName))
            {
                throw new Exception($"{nameof(HeaderName)} is null or empty");
            };
        }
    }
}
