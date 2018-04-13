using Righthand.Immutable;

namespace Sharp.Redux.Visualizer.States
{
    public readonly struct RootState
    {
        public VisualizerState Visualizer { get; }
        public HubState Hub { get; }

        public RootState(VisualizerState visualizer, HubState hub)
        {
            Visualizer = visualizer;
            Hub = hub;
        }

        public RootState Clone(Param<VisualizerState>? visualizer = null, Param<HubState>? hub = null)
        {
            return new RootState(visualizer.HasValue ? visualizer.Value.Value : Visualizer,
hub.HasValue ? hub.Value.Value : Hub);
        }
    }
}
