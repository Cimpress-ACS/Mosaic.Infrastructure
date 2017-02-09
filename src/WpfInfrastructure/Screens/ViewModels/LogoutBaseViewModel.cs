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
using System.Windows.Input;
using Caliburn.Micro;
using Centigrade.Kit.StateMachine;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    public class LogoutBaseViewModel : Screen
    {
        private readonly StateMachine stateMachine;
        private readonly ICommand loggingOutCommand;

        public LogoutBaseViewModel(StateMachine stateMachine, ICommand loggingOutCommand)
        {
            this.stateMachine = stateMachine;
            this.loggingOutCommand = loggingOutCommand;
            this.stateMachine.StateChanged += stateMachine_StateChanged;
        }

        void stateMachine_StateChanged(object sender, StateTransitioningEventArgs e)
        {
            Console.WriteLine("StateChanged to "+e.ToState);
        }

        #region Properties

        public StateMachine StateMachine
        {
            get
            {
                return this.stateMachine;
            }
        }

        #endregion Properties

        #region States

        public class LogoutDefaultState : State { }

        public IState LogoutDefaultViewModelState
        {
            get { return StateMachine.GetState<LogoutDefaultState>(); }
        }

        #endregion States

        #region Commands

        /// <summary>
        /// Gets the command that is executed, when the logout is completed, to return to the login screen.
        /// </summary>
        /// <value>
        /// The logging out command.
        /// </value>
        public ICommand LoggingOutCommand
        {
            get { return this.loggingOutCommand; }
        }

        #endregion Commands
    }
}
