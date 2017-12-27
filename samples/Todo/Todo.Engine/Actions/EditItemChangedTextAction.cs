using Sharp.Redux;

namespace Todo.Engine.Actions
{
    public class EditItemChangedTextAction: ReduxAction
    {
        public  string Text { get; }
        public EditItemChangedTextAction(string text)
        {
            Text = text;
        }
    }
}
