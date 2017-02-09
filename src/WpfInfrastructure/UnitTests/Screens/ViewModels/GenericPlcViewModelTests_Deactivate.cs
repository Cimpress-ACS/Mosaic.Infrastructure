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
    public class GenericPlcViewModelTests_Deactivate : GenericPlcViewModelTests_Base
    {
        [Test]
        public void OnTreeWithControllersInManualMode_ShouldAskUserAboutAutoMode()
        {
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel(_randomModule);
            viewModel.MasterTree.Add(ControllerInManualMode());
            viewModel.Deactivate();
            _askUser.Verify(a => a.AskYesOrNoQuestion(It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void WithCloseTrue_OnTreeWithControllersInManualMode_ShouldNotAskUser()
        {
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel(_randomModule);
            viewModel.MasterTree.Add(ControllerInManualMode());
            viewModel.Deactivate(close:true);
            _askUser.Verify(a => a.AskYesOrNoQuestion(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void OnTreeWithManualModeControllerAndUserSettingToAuto_ShouldSetAllControllersToAutoMode()
        {
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel(_randomModule);
            viewModel.MasterTree.Add(ControllerInManualMode());
            _askUser.Setup(a => a.AskYesOrNoQuestion(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            viewModel.Deactivate();

            _informationProvider.Verify(i => i.SetAllControllersToMode(_randomModule, Controller.Mode.Auto));
        }

        [Test]
        public void OnTreeWithManualModeControllerAndUserNotSettingToAuto_ShouldNotSetAllControllersToAutoMode()
        {
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel(_randomModule);
            viewModel.MasterTree.Add(ControllerInManualMode());
            _askUser.Setup(a => a.AskYesOrNoQuestion(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            viewModel.Deactivate();

            _informationProvider.Verify(i => i.SetAllControllersToMode(_randomModule, Controller.Mode.Auto), Times.Never);
        }

        [Test]
        public void OnTreeWithoutManualModeController_ShouldNotAskUser()
        {
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel();
            viewModel.Deactivate();
            _askUser.Verify(a => a.AskYesOrNoQuestion(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ShouldUnsubscribeFromSelectedItem()
        {
            int randomController = CreateRandom.Int();
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel(moduleName:_randomModule);
            viewModel.SelectedItem = MasterDetailViewModel(_randomModule, randomController);
            viewModel.Deactivate();
            _informationProvider.Verify(i => i.UnsubscribeFromControllerChanges(_randomModule, randomController, It.IsAny<Action<Controller>>()));
        }

        [Test]
        public void ShouldUnsubscribeFromTreeUpdates()
        {
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel(moduleName: _randomModule);
            viewModel.Deactivate();
            _informationProvider.Verify(i => i.UnsubscribeFromTreeUpdate(_randomModule, It.IsAny<Action<Controller>>()));
        }

        private MasterDetailViewModel ControllerInManualMode()
        {
            var controller = new Mock<MasterDetailViewModel>();
            controller.Setup(c => c.HasControllerInMode(Controller.Mode.Manual)).Returns(true);
            return controller.Object;
        }
    }
}
