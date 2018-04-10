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
                SharpReduxSender.Settings.LogLevel = Settings.LogLevel;
                SharpReduxSender.Start(Settings.Token, Settings.HubServer, sourceDispatcher,
                    new Sharp.Redux.HubClient.Models.SessionInfo("1.0.0", "Miha"), 
                    new SharpReduxSenderSettings(Settings.PersistData, Settings.IncludeState, dataFile: Settings.DataFile));
            }
        }
    }
}
