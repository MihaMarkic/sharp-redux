using Righthand.Immutable;

namespace Sharp.Redux.Playground.Engine.State
{
    public class FirstPageState
    {
        public string Input { get; }
        public string Output { get; }

        public FirstPageState(string input, string output)
        {
            Input = input;
            Output = output;
        }

        public FirstPageState Clone(Param<string>? input = null, Param<string>? output = null)
        {
            return new FirstPageState(input.HasValue ? input.Value.Value : Input,
output.HasValue ? output.Value.Value : Output);
        }
    }
}
