using Righthand.Immutable;

namespace Sharp.Redux.Visualizer.States
{
    public readonly struct RootState
    {
        public VisualizerState Visualizer { get; }

        public RootState(VisualizerState visualizerState)
        {
            Visualizer = visualizerState;
        }

        public RootState Clone(Param<VisualizerState>? visualizerState = null)
        {
            return new RootState(visualizerState.HasValue ? visualizerState.Value.Value : Visualizer);
        }
    }
}
