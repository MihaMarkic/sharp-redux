using Sharp.Redux;

namespace Todo.Engine.Actions
{
    public class AddItemAction: ReduxAction
    {
        public int Key { get; }
        public string Text { get; }
        public AddItemAction(int key, string text)
        {
            Key = key;
            Text = text;
        }
    }
}
