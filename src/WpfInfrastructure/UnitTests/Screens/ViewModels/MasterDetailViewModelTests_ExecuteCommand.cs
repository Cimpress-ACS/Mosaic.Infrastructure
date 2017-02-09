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
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.UnitTests.Screens.ViewModels
{
    [TestFixture]
    public class MasterDetailViewModelTests_ExecuteCommand : MasterDetailViewModelTests_Base
    {
        [Test]
        public void ShouldCommandControllerToExecuteCommand()
        {
            string randomCommand = CreateRandom.String();
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel(_randomModule, _randomController);
            viewModel.Command.Execute(randomCommand);
            _commander.Verify(c => c.ExecuteCommand(_randomModule, _randomController, randomCommand));
        }

        [Test]
        public void WithAnyOtherObject_ShouldCommandNothing()
        {
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.Command.Execute(new object());
            _commander.Verify(c => c.ExecuteCommand(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void WithNull_ShouldCommandNothing()
        {
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.Command.Execute(null);
            _commander.Verify(c => c.ExecuteCommand(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void OnCommanderThatThrows_ShouldDisplayErrorScreen()
        {
            string randomError = CreateRandom.String();
            _commander.Setup(c => c.ExecuteCommand(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).Throws(new Exception(randomError));
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.Command.Execute("Stop");
            _states.Verify(s => s.ChangeToErrorState(It.Is<string>(e => e.Contains(randomError))));
        }
    }
}
