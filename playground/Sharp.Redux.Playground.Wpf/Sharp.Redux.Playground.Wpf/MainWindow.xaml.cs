using Autofac;
using Sharp.Redux.Playground.Engine;
using Sharp.Redux.Playground.Engine.ViewModels;
using Sharp.Redux.Visualizer;
using Sharp.Redux.Visualizer.Wpf.Views;
using System;
using System.Windows;

namespace Sharp.Redux.Playground.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; private set; }
        public MainWindow()
        {
            InitializeComponent();

            IoC.Init(); 
            // connect visualizer
            ReduxVisualizer.Init(IoCRegistrar.Container.Resolve<IPlaygroundReduxDispatcher>());
            ViewModel = IoCRegistrar.Container.Resolve<MainViewModel>();
            ViewModel.Start();
            DataContext = ViewModel;
            new ReduxVisualizerWindow().Show();
        }

        protected override void OnClosed(EventArgs e)
        {
            IoCRegistrar.Container.Dispose();
            base.OnClosed(e);
        }
    }
}
