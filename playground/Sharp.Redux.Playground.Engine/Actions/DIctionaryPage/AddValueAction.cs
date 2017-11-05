using Righthand.Immutable;

namespace Sharp.Redux.Playground.Engine.Actions.DIctionaryPage
{
    public class AddValueAction: ReduxAction
    {
        public int Key { get; }
        public string Value { get; }

        public AddValueAction(int key, string value) : base()
        {
            Key = key;
            Value = value;
        }

        public AddValueAction Clone(Param<int>? key = null, Param<string>? value = null)
        {
            return new AddValueAction(key.HasValue ? key.Value.Value : Key,
value.HasValue ? value.Value.Value : Value);
        }
    }
}
