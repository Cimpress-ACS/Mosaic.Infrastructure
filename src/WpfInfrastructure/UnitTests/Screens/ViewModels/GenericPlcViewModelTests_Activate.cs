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
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VP.FF.PT.Common.TestInfrastructure;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.UnitTests.Screens.ViewModels
{
    [TestFixture]
    public class GenericPlcViewModelTests_Activate : GenericPlcViewModelTests_Base
    {
        [Test]
        public void ShouldChangeToLoadingState()
        {
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel();
            viewModel.Activate();
            _states.Verify(x => x.ChangeToLoadingState());
        }

        [Test]
        public void OnRandomModule_ShouldGetRootControllerOfRandomModule()
        {
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel(moduleName:_randomModule);
            viewModel.Activate();
            _informationProvider.Verify(x => x.LoadRootController(_randomModule));
        }

        [Test]
        public void OnPreviouslyDeactivatedViewModel_ShouldSubscribeForSelectedItem()
        {
            int randomController = CreateRandom.Int();
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel(moduleName:_randomModule);
            MasterDetailViewModel anyController = MasterDetailViewModel(_randomModule, randomController);
            viewModel.MasterTree.Add(anyController);
            viewModel.SelectedItem = anyController;
            viewModel.Deactivate();
            viewModel.Activate();
            _informationProvider.Verify(x => x.SubscribeForControllerChanges(_randomModule, randomController, It.IsAny<Action<Controller>>()));
        }

        [Test]
        public void OnExistingModule_ShouldChangeToContentState()
        {
            _informationProvider.Setup(x => x.LoadRootController(It.IsAny<string>())).Returns(Task.FromResult(new Controller()));
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel();
            viewModel.Activate();
            _states.Verify(x => x.ChangeToContentState());
        }

        [Test]
        public void OnInformationProviderWithError_ShouldChangeToErrorState()
        {
            _informationProvider.Setup(x => x.LoadRootController(It.IsAny<string>())).Throws(new InvalidOperationException());
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel();
            viewModel.Activate();
            _states.Verify(x => x.ChangeToErrorState(It.IsAny<string>()));
        }

        [Test]
        public void OnInformationProviderWithRandomError_ShouldExposeRandomError()
        {
            string randomError = CreateRandom.String();
            _informationProvider.Setup(x => x.LoadRootController(It.IsAny<string>())).Throws(new InvalidOperationException(randomError));
            TestableGenericPlcViewModel viewModel = CreateTestableGenericPlcViewModel();
            viewModel.MonitorEvents();
            viewModel.Activate();
            _states.Verify(s => s.ChangeToErrorState(It.Is<string>(e => e.Contains(randomError))));
        }
    }
}
