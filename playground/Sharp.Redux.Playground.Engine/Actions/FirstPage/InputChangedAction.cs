using Righthand.Immutable;

namespace Sharp.Redux.Playground.Engine.Actions.FirstPage
{
    public class InputChangedAction: ReduxAction
    {
        public string Value { get; }

        public InputChangedAction(string value)
        {
            Value = value;
        }

        public InputChangedAction Clone(Param<string>? value = null)
        {
            return new InputChangedAction(value.HasValue ? value.Value.Value : Value);
        }
    }
}
