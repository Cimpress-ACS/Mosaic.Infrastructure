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


using System.ComponentModel.Composition;
using Caliburn.Micro;
using Centigrade.Kit.StateMachine;
using VP.FF.PT.Common.WpfInfrastructure.Threading;

namespace VP.FF.PT.Common.WpfInfrastructure.ScreenActivation
{
    [Export(typeof(IProvideStatesForScreenActivation))]
    public class CentigadeStateMachineAdapter : PropertyChangedBase, IProvideStatesForScreenActivation
    {
        private readonly IDispatcher _dispatcher;
        private readonly StateMachine _centigadeStateMachine;
        private string _errorMessage;

        [ImportingConstructor]
        public CentigadeStateMachineAdapter(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            _centigadeStateMachine = new StateMachine();
            _errorMessage = string.Empty;
        }

        public StateMachine CentigadeStateMachine
        {
            get { return _centigadeStateMachine; }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }

        public State Loading
        {
            get { return _centigadeStateMachine.GetState<LoadingState>(); }
        }

        public State Content
        {
            get { return _centigadeStateMachine.GetState<ContentState>(); }
        }

        public State Error
        {
            get { return _centigadeStateMachine.GetState<ErrorState>(); }
        }

        public virtual void ChangeToLoadingState()
        {
            _dispatcher.Dispatch(() => GoToState<LoadingState>());
        }

        public virtual void ChangeToContentState()
        {
            _dispatcher.Dispatch(() => GoToState<ContentState>());
        }

        public virtual void ChangeToErrorState(string errorMessage)
        {
            _errorMessage = errorMessage;
            NotifyOfPropertyChange(() => ErrorMessage);
            _dispatcher.Dispatch(() => GoToState<ErrorState>());
        }

        private void GoToState<TState>()
            where TState : State, new()
        {
            _centigadeStateMachine.GoToState<TState>();
        }

        private class LoadingState : State { }
        private class ContentState : State { }
        private class ErrorState : State { }
    }
}
