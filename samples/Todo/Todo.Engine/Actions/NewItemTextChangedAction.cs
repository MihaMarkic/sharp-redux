using Sharp.Redux;

namespace Todo.Engine.Actions
{
    public class NewItemTextChangedAction: ReduxAction
    {
        public string Text { get; }

        public NewItemTextChangedAction(string text) : base()
        {
            Text = text;
        }
    }
}
