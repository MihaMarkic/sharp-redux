using Righthand.Immutable;

namespace Sharp.Redux.Visualizer.States
{
    public class RootState
    {
        public Step[] Steps { get; }

        public RootState(Step[] steps)
        {
            Steps = steps;
        }

        public RootState Clone(Param<Step[]>? steps = null)
        {
            return new RootState(steps.HasValue ? steps.Value.Value : Steps);
        }
    }
}
