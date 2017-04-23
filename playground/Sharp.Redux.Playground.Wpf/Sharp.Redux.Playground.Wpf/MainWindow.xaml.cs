using Autofac;
using Sharp.Redux.Playground.Engine;
using Sharp.Redux.Playground.Engine.ViewModels;
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

            ViewModel = IoCRegistrar.Container.Resolve<MainViewModel>();
            ViewModel.Start();
            DataContext = ViewModel;
        }
    }
}
