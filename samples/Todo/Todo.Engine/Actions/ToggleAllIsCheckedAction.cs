using Sharp.Redux;

namespace Todo.Engine.Actions
{
    public class ToggleAllIsCheckedAction: ReduxAction
    {
        public  bool IsChecked { get; }
        public ToggleAllIsCheckedAction(bool isChecked)
        {
            IsChecked = isChecked;
        }
    }
}
