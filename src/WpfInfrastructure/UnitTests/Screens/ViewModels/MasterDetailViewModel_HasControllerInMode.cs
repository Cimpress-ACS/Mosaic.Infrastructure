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


using FluentAssertions;
using NUnit.Framework;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.UnitTests.Screens.ViewModels
{
    [TestFixture]
    public class MasterDetailViewModel_HasControllerInMode : MasterDetailViewModelTests_Base
    {
        [Test]
        public void WithAuto_OnViewModelWithControllerInAutoMode_ShouldReturnTrue()
        {
            _innerController.ControllerMode = Controller.Mode.Auto;
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.HasControllerInMode(Controller.Mode.Auto).Should().BeTrue();
        }

        [Test]
        public void WithAuto_OnViewModelwithControllerInManualMode_ShouldReturnFalse()
        {
            _innerController.ControllerMode = Controller.Mode.Manual;
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.HasControllerInMode(Controller.Mode.Auto).Should().BeFalse();
        }

        [Test]
        public void WithManual_OnViewModelWithControllerInManualMode_ShouldReturnTrue()
        {
            _innerController.ControllerMode = Controller.Mode.Manual;
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.HasControllerInMode(Controller.Mode.Manual).Should().BeTrue();
        }

        [Test]
        public void WithAuto_OnViewModelWithControllerInManualModelAndChildInAutoMode_ShouldReturnTrue()
        {
            _innerController.ControllerMode = Controller.Mode.Manual;
            MasterDetailViewModel child = CreateMasterDetailViewModel(new Controller {ControllerMode = Controller.Mode.Auto});
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.Childs.Add(child);

            viewModel.HasControllerInMode(Controller.Mode.Auto).Should().BeTrue();
        }
    }
}
