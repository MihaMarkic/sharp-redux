using Sharp.Redux.HubClient.Core;
using System;

namespace Todo.Engine
{
    /// <summary>
    /// Settings.
    /// </summary>
    /// <remarks>
    /// To override default values, add a (git ignored) class Settings.nogit.cs and 
    /// override values in static constructor.
    /// </remarks>
    public static partial class Settings
    {
        public static bool IsHubEnabled { get; private set; }
        public static Uri HubServer { get; private set; }
        public static string Token { get; private set; }
        public static bool PersistData { get; private set; }
        public static string DataFile { get; private set; }
        public static bool IncludeState { get; private set; }
        public static LogLevel LogLevel { get; private set; }
    }
}
