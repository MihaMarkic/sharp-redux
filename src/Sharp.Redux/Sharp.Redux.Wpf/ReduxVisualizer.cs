using Sharp.Redux.Wpf.ViewModels;
using System.Collections.ObjectModel;

namespace Sharp.Redux.Wpf
{
    public class ReduxVisualizer
    {
        private IReduxDispatcher dispatcher;
        public static ReduxVisualizer Default { get; private set; }
        public ObservableCollection<StepViewModel> Steps { get; private set; }

        public static void Init(IReduxDispatcher dispatcher)
        {
            Default = new ReduxVisualizer(dispatcher);
        }

        public ReduxVisualizer(IReduxDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            dispatcher.StateChanged += Dispatcher_StateChanged;
            Steps = new ObservableCollection<StepViewModel>();
        }

        private void Dispatcher_StateChanged(object sender, StateChangedEventArgs e)
        {
            Steps.Add(new StepViewModel(e.Action, dispatcher.State));
        }
    }
}
