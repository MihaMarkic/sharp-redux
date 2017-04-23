using Righthand.Immutable;

namespace Sharp.Redux.Visualizer.States
{
    public class RootState
    {
        public Step[] Steps { get; }
        public Step SelectedStep { get; }

        public RootState(Step[] steps, Step selectedStep)
        {
            Steps = steps;
            SelectedStep = selectedStep;
        }

        public RootState Clone(Param<Step[]>? steps = null, Param<Step>? selectedStep = null)
        {
            return new RootState(steps.HasValue ? steps.Value.Value : Steps,
selectedStep.HasValue ? selectedStep.Value.Value : SelectedStep);
        }
    }
}
