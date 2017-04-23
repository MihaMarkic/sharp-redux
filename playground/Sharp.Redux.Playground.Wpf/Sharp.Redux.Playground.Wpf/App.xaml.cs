using Autofac;
using Sharp.Redux.Playground.Engine;
using Sharp.Redux.Visualizer;
using System.Windows;

namespace Sharp.Redux.Playground.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            IoC.Init();
            // connect visualizer
            ReduxVisualizer.Init(
                IoCRegistrar.Container.Resolve<IPlaygroundReduxDispatcher>(),
                new string[] { "Sharp.Redux.Playground.Engine.Actions" });
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            IoCRegistrar.Container.Dispose();
            base.OnExit(e);
        }
    }
}
