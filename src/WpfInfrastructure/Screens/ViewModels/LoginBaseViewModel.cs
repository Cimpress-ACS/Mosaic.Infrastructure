/* Copyright 2017 Cimpress

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */


using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Caliburn.Micro;
using Centigrade.Kit.StateMachine;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    public abstract class LoginBaseViewModel : Screen
    {
        private readonly ICommand _loginCommand;
        private readonly StateMachine _stateMachine;

        protected LoginBaseViewModel(StateMachine stateMachine, ICommand loginCommand)
        {
            _stateMachine = stateMachine;
            _loginCommand = loginCommand;
            _stateMachine.StateChanged += stateMachine_StateChanged;
        }

        private void stateMachine_StateChanged(object sender, StateTransitioningEventArgs e)
        {
            Console.WriteLine("Login StateChanged To : " + e.ToState);
        }

        #region Properties

        public StateMachine StateMachine
        {
            get { return _stateMachine; }
        }

        public abstract ObservableCollection<string> UserNames { get; set; }
        public abstract ObservableCollection<User> RecentUsers { get; set; }
        public abstract string UserName { get; set; }
        public abstract string Password { get; set; }

        #endregion Properties

        #region States

        public IState LoginDefaultViewModelState
        {
            get { return StateMachine.GetState<LoginDefaultState>(); }
        }

        public class LoginDefaultState : State
        {
        }

        #endregion States

        #region Commands

        public ICommand ResetCommand
        {
            get
            {
                return StateMachine.GetStateTransitionCommand<LoginDefaultState, LoginDefaultState, object>(
                    p => OnReset(),  
                    p => OnCanExecute());
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand;
            }
        }

        protected virtual bool OnCanExecute()
        {
            return true;
        }

        protected virtual void OnReset()
        {
        }

        #endregion Commands
    }
}
