using Righthand.Immutable;

namespace Sharp.Redux.HubClient.Models
{
    public readonly struct EnvironmentInfo
    {
        public string AppVersion { get; }
        public string UserName { get; }

        public EnvironmentInfo(string appVersion, string userName)
        {
            AppVersion = appVersion;
            UserName = userName;
        }

        public EnvironmentInfo Clone(Param<string>? appVersion = null, Param<string>? userName = null)
        {
            return new EnvironmentInfo(appVersion.HasValue ? appVersion.Value.Value : AppVersion,
userName.HasValue ? userName.Value.Value : UserName);
        }
    }
}
