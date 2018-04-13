using Righthand.Immutable;

namespace Sharp.Redux.Visualizer.States
{
    public readonly struct VisualizerState
    {
        public Step[] Steps { get; }
        public Step SelectedStep { get; }

        public VisualizerState(Step[] steps, Step selectedStep)
        {
            Steps = steps;
            SelectedStep = selectedStep;
        }

        public VisualizerState Clone(Param<Step[]>? steps = null, Param<Step>? selectedStep = null)
        {
            return new VisualizerState(steps.HasValue ? steps.Value.Value : Steps,
selectedStep.HasValue ? selectedStep.Value.Value : SelectedStep);
        }
    }
}
