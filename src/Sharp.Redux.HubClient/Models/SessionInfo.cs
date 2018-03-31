using Righthand.Immutable;

namespace Sharp.Redux.HubClient.Models
{
    public readonly struct SessionInfo
    {
        public string AppVersion { get; }
        public string UserName { get; }

        public SessionInfo(string appVersion, string userName)
        {
            AppVersion = appVersion;
            UserName = userName;
        }

        public SessionInfo Clone(Param<string>? appVersion = null, Param<string>? userName = null)
        {
            return new SessionInfo(appVersion.HasValue ? appVersion.Value.Value : AppVersion,
userName.HasValue ? userName.Value.Value : UserName);
        }
    }
}
