﻿using System;
using Autofac;
using Sharp.Redux.Playground.Engine.Core;
using Sharp.Redux.Playground.Engine.Actions.FirstPage;

namespace Sharp.Redux.Playground.Engine.ViewModels
{
    public class FirstPageViewModel: PageViewModel
    {
        private string input;
        private string output;
        public RelayCommand Click { get; }
        public FirstPageViewModel(ILifetimeScope lifetimeScope, IPlaygroundReduxDispatcher dispatcher): base(lifetimeScope, dispatcher)
        {
            Click = new RelayCommand(ClickExecute);
        }
        public void ClickExecute()
        {
            dispatcher.Dispatch(new InputChangedAction(Input));
        }

        public override void StateChanged()
        {
            State.FirstPageState state = dispatcher.State.FirstPage;
            Input = state.Input;
            Output = state.Output;
        }

        #region Properties
        public string Input
        {
            get { return input; }
            set
            {
                if (input != value)
                {
                    input = value;
                    OnPropertyChanged();
                }
            }
        }
        public string Output
        {
            get
            {
                return output;
            }
            set
            {
                if (output != value)
                {
                    output = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion
    }
}
