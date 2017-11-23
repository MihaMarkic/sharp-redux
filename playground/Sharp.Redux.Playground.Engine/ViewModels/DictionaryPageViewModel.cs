using Autofac;
using Sharp.Redux.Playground.Engine.Actions.DictionaryPage;
using Sharp.Redux.Playground.Engine.Core;
using System.Collections.ObjectModel;
using DictionaryItem = System.Collections.Generic.KeyValuePair<int, Sharp.Redux.Playground.Engine.ViewModels.DictionaryItemViewModel<string>>;
using System.Runtime.CompilerServices;

namespace Sharp.Redux.Playground.Engine.ViewModels
{
    public class DictionaryPageViewModel: PageViewModel
    {
        public ObservableCollection<DictionaryItem> Data { get; } = new ObservableCollection<DictionaryItem>();
        public RelayCommand AddItem { get; }
        public RelayCommand RemoveItem { get; }
        DictionaryItem? selectedItem;
        int actionIndex = 0;
        public DictionaryPageViewModel(ILifetimeScope lifetimeScope, IPlaygroundReduxDispatcher dispatcher) : base(lifetimeScope, dispatcher)
        {
            AddItem = new RelayCommand(AddItemExecute);
            RemoveItem = new RelayCommand(RemoveItemExecute, () => !(SelectedItem is null));
        }
        void AddItemExecute()
        {
            dispatcher.Dispatch(new AddValueAction(actionIndex, actionIndex.ToString()));
            actionIndex++;
        }
        void RemoveItemExecute()
        {
            dispatcher.Dispatch(new RemoveValueAction(SelectedItem.Value.Key));
        }

        public override void StateChanged()
        {
            var state = dispatcher.State.DictionaryPage;
            ReduxMerger.MergeDictionary(state.Dictionary, Data, text => new DictionaryItemViewModel<string>(text));
        }
        protected override void OnPropertyChanged([CallerMemberName] string name = null)
        {
            switch (name)
            {
                case nameof(SelectedItem):
                    RemoveItem.RaiseCanExecuteChanged();
                    break;
            }
            base.OnPropertyChanged(name);
        }

        #region Properties
        public DictionaryItem? SelectedItem
        {
            get => selectedItem;
            set
            {
                if (selectedItem?.Key == value?.Key)
                {
                    return;
                }
                selectedItem = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
