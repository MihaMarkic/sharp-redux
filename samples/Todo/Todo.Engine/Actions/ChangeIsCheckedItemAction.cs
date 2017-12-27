using Sharp.Redux;

namespace Todo.Engine.Actions
{
    public class ChangeIsCheckedItemAction: ReduxAction
    {
        public int Key { get; }
        public bool IsChecked { get; }
        public ChangeIsCheckedItemAction(int key, bool isChecked)
        {
            Key = key;
            IsChecked = isChecked;
        }
    }
}
