using Sharp.Redux;
using Sharp.Redux.HubClient;

namespace Todo.Engine
{
    public static class Bootstrapper
    {
        public static void Init(IReduxDispatcher sourceDispatcher)
        {
            if (Settings.IsHubEnabled)
            {
                SharpReduxManager.Settings.LogLevel = Settings.LogLevel;
                SharpReduxManager.Start(Settings.UploadToken, Settings.DownloadToken, Settings.HubServer, sourceDispatcher,
                    new Sharp.Redux.HubClient.Models.EnvironmentInfo("1.0.0", "Miha"), 
                    new SharpReduxManagerSettings(Settings.PersistData, Settings.IncludeState, dataFile: Settings.DataFile));
            }
        }
    }
}
