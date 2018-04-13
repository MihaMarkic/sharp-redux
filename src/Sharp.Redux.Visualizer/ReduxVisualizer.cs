using Sharp.Redux.Visualizer.ViewModels;

namespace Sharp.Redux.Visualizer
{
    public static class ReduxVisualizer
    {
        public static MainViewModel MainViewModel { get; private set; }
        public static string[] IgnoredNamespacePrefixes { get; private set; }
        public static void Init(IReduxDispatcher sourceDispatcher, string[] ignoredNamespacePrefixes = null)
        {
            IgnoredNamespacePrefixes = ignoredNamespacePrefixes ?? new string[0];
            VisualizerDispatcher.Init();
            MainViewModel = new MainViewModel(sourceDispatcher);
            VisualizerDispatcher.Default.Start();
        }
    }
}
