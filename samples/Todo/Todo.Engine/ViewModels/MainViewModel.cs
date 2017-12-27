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
        public MainViewModel(ITodoReduxDispatcher dispatcher, Func<TodoItem, TodoItemViewModel> todoItemViewModelFactory)
        {
            this.dispatcher = dispatcher;
            this.todoItemViewModelFactory = todoItemViewModelFactory;
            dispatcher.StateChanged += UpdateReduxState;
            SetFilterCommand = new RelayCommand<ItemsFilter>(SetFilter, f => !State.IsEditing);
            AddItemCommand = new RelayCommand(AddItem, () => !string.IsNullOrEmpty(State.NewItemText));
            RemoveCompletedCommand = new RelayCommand(RemoveCompleted, () => State.HasCompleted && !State.IsEditing);
        }

        public void Start()
        {
            dispatcher.Start();
            UpdateReduxState(this, EventArgs.Empty);
        }
        RootState State => dispatcher.State;
        void UpdateReduxState(object sender, EventArgs e)
        {
            isUpdatingState = true;
            try
            {
                AllChecked = State.AllChecked;
                NewItemText = State.NewItemText;
                int itemsLeft = State.Items.Count(i => !i.IsChecked);
                ItemsLeftInfo = $"{itemsLeft} item{(itemsLeft != 1 ? "s" : "")} left";
                ReduxMerger.MergeList<int, TodoItem, TodoItemViewModel>(State.FilteredItems, Items, i => todoItemViewModelFactory(i));
                Filter = State.Filter;
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
            int maxItemId = State.Items.Count > 0 ? dispatcher.State.Items.Max(i => i.Key) : 0;
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
                        break;                }
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
