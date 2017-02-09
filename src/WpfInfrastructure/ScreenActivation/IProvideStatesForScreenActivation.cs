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


using System.ComponentModel;
using Centigrade.Kit.StateMachine;

namespace VP.FF.PT.Common.WpfInfrastructure.ScreenActivation
{
    public interface IProvideStatesForScreenActivation : INotifyPropertyChanged
    {
        StateMachine CentigadeStateMachine { get; }

        string ErrorMessage { get; }

        State Loading { get; }

        State Content { get; }

        State Error { get; }

        void ChangeToLoadingState();

        void ChangeToContentState();

        void ChangeToErrorState(string errorMessage);
    }

    public class ScreenActivationNullObject : CentigadeStateMachineAdapter
    {
        public ScreenActivationNullObject() : base(null)
        {
        }

        public override void ChangeToLoadingState()
        {
        }

        public override void ChangeToContentState()
        {
        }

        public override void ChangeToErrorState(string errorMessage)
        {
        }
    }
}
