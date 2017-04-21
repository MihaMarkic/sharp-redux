using Autofac;
using Sharp.Redux.Playground.Engine.Core;
using Sharp.Redux.Playground.Engine.State;

namespace Sharp.Redux.Playground.Engine.ViewModels
{
    public class MainViewModel: BaseViewModel
    {
        private readonly IPlaygroundReduxDispatcher dispatcher;
        private NavigationState navigationState;
        PageViewModel content;

        public MainViewModel(IPlaygroundReduxDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            dispatcher.StateChanged += (s, e) => UpdateReduxState();
        }

        public void Start()
        {
            dispatcher.Start();
            UpdateReduxState();
        }

        public void UpdateReduxState()
        {
            var newState = dispatcher.State.Navigation;
            if (!ReferenceEquals(newState, navigationState))
            {
                PageViewModel page = Content;
                if (navigationState?.Page != newState.Page)
                {
                    var lifetimeScope = IoCRegistrar.Container.BeginLifetimeScope();
                    switch (newState.Page)
                    {
                        case NavigationPage.FirstPage:
                            page = lifetimeScope.Resolve<FirstPageViewModel>();
                            break;
                    }
                }
                if (page != Content)
                {
                    var oldContent = Content;
                    Content = page;
                    oldContent?.Dispose();
                }
                navigationState = newState;
            }
        }

        #region Properties
        public PageViewModel Content
        {
            get { return content; }
            set
            {
                if (content != value)
                {
                    content = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}
