using Sharp.Redux.Visualizer.Reducers;
using Sharp.Redux.Visualizer.States;
using System.Threading.Tasks;

namespace Sharp.Redux.Visualizer
{
    public class VisualizerDispatcher : ReduxDispatcher<RootState, RootReducer>
    {
        public static VisualizerDispatcher Default { get; private set; }
        private VisualizerDispatcher(RootState initialState, RootReducer reducer, TaskScheduler notificationScheduler) : 
            base(initialState, reducer, notificationScheduler)
        {}

        public static void Init()
        {
            RootState initialState = new RootState(new VisualizerState(steps: new Step[0], selectedStep: null));
            Default = new VisualizerDispatcher(initialState, new RootReducer(), TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
