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
using Moq;
using NUnit.Framework;
using VP.FF.PT.Common.TestInfrastructure;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.UnitTests.Screens.ViewModels
{
    [TestFixture]
    public class MasterDetailViewModelTests : MasterDetailViewModelTests_Base
    {
        [Test]
        public void ChangeSimulationMode_OnSimulated_ShouldDeactivateSimulationOnCommander()
        {
            _innerController.IsSimulation = true;
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.ChangeSimulationMode(false);
            _commander.Verify(c => c.DeactivateSimulation(_randomModule, _randomController,false));
        }

        [Test]
        public void ChangeSimulationMode_OnReal_ShouldActivateSimulationOnCommander()
        {
            _innerController.IsSimulation = false;
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.ChangeSimulationMode(false);
            _commander.Verify(c => c.ActivateSimulation(_randomModule, _randomController, false));
        }

        [Test]
        public void ChangeSimulationMode_OnFaultingCommander_ShouldDisplayError()
        {
            string randomError = CreateRandom.String();
            _innerController.IsSimulation = false;
            _commander.Setup(c => c.ActivateSimulation(It.IsAny<string>(), It.IsAny<int>(), false)).Throws(new InvalidOperationException(randomError));
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.ChangeSimulationMode(false);
            _states.Verify(s => s.ChangeToErrorState(It.Is<string>(e => e.Contains(randomError))));
        }

        [Test]
        public void ChangeMode_OnControllerInAutoMode_ShouldDelgateToCommanderWithManual()
        {
            _innerController.ControllerMode = Controller.Mode.Auto;
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.ChangeMode();
            _commander.Verify(c => c.SetControllerToMode(_randomModule, _randomController, Controller.Mode.Manual));
        }

        [Test]
        public void ChangeMode_OnControllerInManualMode_ShouldDelgateToCommanderWithAuto()
        {
            _innerController.ControllerMode = Controller.Mode.Manual;
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.ChangeMode();
            _commander.Verify(c => c.SetControllerToMode(_randomModule, _randomController, Controller.Mode.Auto));
        }

        [Test]
        public void ChangeMode_OnFaultingCommander_ShouldDisplayError()
        {
            string randomError = CreateRandom.String();
            _commander.Setup(c => c.SetControllerToMode(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Controller.Mode>())).Throws(new InvalidOperationException(randomError));
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.ChangeMode();
            _states.Verify(s => s.ChangeToErrorState(It.Is<string>(e => e.Contains(randomError))));
        }
    }
}
