using Autofac;
using Sharp.Redux.Playground.Engine.Actions.DIctionaryPage;
using Sharp.Redux.Playground.Engine.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sharp.Redux.Playground.Engine.ViewModels
{
    public class DictionaryPageViewModel: PageViewModel
    {
        public ObservableCollection<KeyValuePair<int, DictionaryItemViewModel<string>>> Data { get; } 
            = new ObservableCollection<KeyValuePair<int, DictionaryItemViewModel<string>>>();
        public RelayCommand AddItem { get; }
        int actionIndex = 0;
        public DictionaryPageViewModel(ILifetimeScope lifetimeScope, IPlaygroundReduxDispatcher dispatcher) : base(lifetimeScope, dispatcher)
        {
            AddItem = new RelayCommand(AddItemExecute);
        }
        void AddItemExecute()
        {
            dispatcher.Dispatch(new AddValueAction(actionIndex, actionIndex.ToString()));
            actionIndex++;
        }

        public override void StateChanged()
        {
            var state = dispatcher.State.DictionaryPage;
            ReduxMerger.MergeDictionary(state.Dictionary, Data, text => new DictionaryItemViewModel<string>(text));
        }
    }
}
