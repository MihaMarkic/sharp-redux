using Sharp.Redux;
using Todo.Engine.Actions;
using Todo.Engine.Core;
using Todo.Engine.Models;

namespace Todo.Engine.ViewModels
{
    public class TodoItemViewModel: NotificableObject, IBoundViewModel<TodoItem>
    {
        readonly ITodoReduxDispatcher dispatcher;
        public TodoItem State { get; private set; }
        public bool IsChecked { get; set; }
        public string Text { get; set; }
        public bool IsEditing { get; set; }
        public string EditText { get; set; }
        public RelayCommand StartItemEditCommand { get; }
        public RelayCommand EndItemEditCommand { get; }
        public RelayCommand CancelItemEditCommand { get; }
        bool isUpdatingState;
        public TodoItemViewModel(ITodoReduxDispatcher dispatcher, TodoItem state)
        {
            this.dispatcher = dispatcher;
            Update(state);
            StartItemEditCommand = new RelayCommand(StartItemEdit);
            EndItemEditCommand = new RelayCommand(EndItemEdit);
            CancelItemEditCommand = new RelayCommand(CancelItemEdit);
        }
        public void Update(TodoItem state)
        {
            isUpdatingState = true;
            try
            {
                State = state;
                Text = state.Text;
                IsChecked = state.IsChecked;
                IsEditing = state.IsEditing;
                EditText = state.EditText;
            }
            finally
            {
                isUpdatingState = false;
            }
        }
        void StartItemEdit()
        {
            dispatcher.Dispatch(new StartEditItemAction(State.Key));
        }
        void EndItemEdit()
        {
            dispatcher.Dispatch(new EndEditItemAction());
        }
        void CancelItemEdit()
        {
            dispatcher.Dispatch(new CancelEditItemAction());
        }
        protected override void OnPropertyChanged(string name)
        {
            if (!isUpdatingState)
            {
                switch(name)
                {
                    case nameof(IsChecked):
                        dispatcher.Dispatch(new ChangeIsCheckedItemAction(State.Key, IsChecked));
                        break;
                    case nameof(EditText):
                        dispatcher.Dispatch(new EditItemChangedTextAction(EditText));
                        break;
                }
            }
            base.OnPropertyChanged(name);
        }
    }
}
