using Sharp.Redux;

namespace Todo.Engine.Actions
{
    public class StartEditItemAction: ReduxAction
    {
        public int Key { get; }
        public StartEditItemAction(int key)
        {
            Key = key;
        }
    }
}
