using Sharp.Redux;
using Todo.Engine.Core;
using Righthand.Immutable;

namespace Todo.Engine.Actions
{
    public class SetFilterAction: ReduxAction
    {
        public ItemsFilter Filter { get; }

        public SetFilterAction(ItemsFilter filter) : base()
        {
            Filter = filter;
        }
    }
}
