using Autofac;

namespace Sharp.Redux.Playground.Engine.ViewModels
{
    public abstract class PageViewModel: BaseViewModel
    {
        protected readonly IPlaygroundReduxDispatcher dispatcher;
        protected readonly ILifetimeScope lifetimeScope;
        public PageViewModel(ILifetimeScope lifetimeScope, IPlaygroundReduxDispatcher dispatcher)
        {
            this.lifetimeScope = lifetimeScope;
            this.dispatcher = dispatcher;
            dispatcher.StateChanged += Dispatcher_StateChanged;
        }

        private void Dispatcher_StateChanged(object sender, System.EventArgs e)
        {
            StateChanged();
        }

        public abstract void StateChanged();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dispatcher.StateChanged -= Dispatcher_StateChanged;
                lifetimeScope?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
