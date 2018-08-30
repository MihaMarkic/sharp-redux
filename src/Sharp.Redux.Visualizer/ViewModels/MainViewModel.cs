using Sharp.Redux.Visualizer.Core;
using Sharp.Redux.Visualizer.States;

namespace Sharp.Redux.Visualizer.ViewModels
{
    public class MainViewModel : NotificableObject
    {
        readonly VisualizerDispatcher dispatcher = VisualizerDispatcher.Default;
        RootState state;
        public VisualizerViewModel Visualizer { get; }
        public MainViewModel(IReduxDispatcher sourceDispatcher)
        {
            Visualizer = new VisualizerViewModel(sourceDispatcher);
            dispatcher.StateChanged += Default_StateChanged;
        }
        void Default_StateChanged(object sender, StateChangedEventArgs<RootState> e)
        {
            state = e.State;
            Visualizer.StateChanged(state.Visualizer);
        }
    }
}
