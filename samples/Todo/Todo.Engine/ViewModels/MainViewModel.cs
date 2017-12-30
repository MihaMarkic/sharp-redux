using Sharp.Redux;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Todo.Engine.Actions;
using Todo.Engine.Core;
using Todo.Engine.Models;
using Todo.Engine.States;

namespace Todo.Engine.ViewModels
{
    public class MainViewModel: NotificableObject
    {
        readonly ITodoReduxDispatcher dispatcher;
        readonly Func<TodoItem, TodoItemViewModel> todoItemViewModelFactory;
        public bool AllChecked { get; set; }
        public string NewItemText { get; set; }
        public  string ItemsLeftInfo { get; private set; }
        public ObservableCollection<TodoItemViewModel> Items { get; } = new ObservableCollection<TodoItemViewModel>();
        public ItemsFilter Filter { get; set; }
        public RelayCommand<ItemsFilter> SetFilterCommand { get; }
        public RelayCommand AddItemCommand { get; }
        public RelayCommand RemoveCompletedCommand { get; }
        bool isUpdatingState;
        RootState state;
        public MainViewModel(ITodoReduxDispatcher dispatcher, Func<TodoItem, TodoItemViewModel> todoItemViewModelFactory)
        {
            this.dispatcher = dispatcher;
            this.todoItemViewModelFactory = todoItemViewModelFactory;
            dispatcher.StateChanged += UpdateReduxState;
            SetFilterCommand = new RelayCommand<ItemsFilter>(SetFilter, f => !state?.IsEditing ?? false);
            AddItemCommand = new RelayCommand(AddItem, () => !string.IsNullOrEmpty(state?.NewItemText));
            RemoveCompletedCommand = new RelayCommand(RemoveCompleted, () => (state?.HasCompleted ?? false) && !(state?.IsEditing ?? false));
        }

        public void Start()
        {
            dispatcher.Start();
        }
        void UpdateReduxState(object sender, StateChangedEventArgs<RootState> e)
        {
            isUpdatingState = true;
            try
            {
                state = e.State;
                AllChecked = e.State.AllChecked;
                NewItemText = e.State.NewItemText;
                int itemsLeft = e.State.Items.Count(i => !i.IsChecked);
                ItemsLeftInfo = $"{itemsLeft} item{(itemsLeft != 1 ? "s" : "")} left";
                ReduxMerger.MergeList<int, TodoItem, TodoItemViewModel>(e.State.FilteredItems, Items, i => todoItemViewModelFactory(i));
                Filter = e.State.Filter;
                AddItemCommand.RaiseCanExecuteChanged();
                RemoveCompletedCommand.RaiseCanExecuteChanged();
                SetFilterCommand.RaiseCanExecuteChanged();
            }
            finally
            {
                isUpdatingState = false;
            }
        }
        void SetFilter(ItemsFilter filter)
        {
            dispatcher.Dispatch(new SetFilterAction(filter));
        }
        void AddItem()
        {
            int maxItemId = Items.Count > 0 ? Items.Max(i => i.State.Key) : 0;
            dispatcher.Dispatch(new AddItemAction(maxItemId + 1, NewItemText));
        }
        void RemoveCompleted()
        {
            dispatcher.Dispatch(new RemoveCompletedAction());
        }
        protected override void OnPropertyChanged(string name)
        {
            if (!isUpdatingState)
            {
                switch (name)
                {
                    case nameof(NewItemText):
                        dispatcher.Dispatch(new NewItemTextChangedAction(NewItemText));
                        break;
                    case nameof(AllChecked):
                        dispatcher.Dispatch(new ToggleAllIsCheckedAction(AllChecked));
                        break;
                }
            }
            base.OnPropertyChanged(name);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dispatcher.StateChanged -= UpdateReduxState;
            }
            base.Dispose(disposing);
        }
    }
}
