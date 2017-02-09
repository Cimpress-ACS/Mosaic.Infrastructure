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
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VP.FF.PT.Common.TestInfrastructure;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.UnitTests.Screens.ViewModels
{
    [TestFixture]
    public class GenericPlcViewModelTests_SelectedItem : GenericPlcViewModelTests_Base
    {
        [Test]
        public void WithRandomController_ShouldSubscribeForChangesOnRandomController()
        {
            string randomModuleName = CreateRandom.String();
            int randomControllerId = CreateRandom.Int();
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel(moduleName:randomModuleName);
            viewModel.SelectedItem = MasterDetailViewModel(randomModuleName, randomControllerId);
            _informationProvider.Verify(x => x.SubscribeForControllerChanges(randomModuleName, randomControllerId, It.IsAny<Action<Controller>>()));
        }

        [Test]
        public void WithRandomController_ShouldImportSelectedController()
        {
            string randomModuleName = CreateRandom.String();
            int randomControllerId = CreateRandom.Int();
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel(moduleName: randomModuleName);
            viewModel.SelectedItem = MasterDetailViewModel(randomModuleName, randomControllerId);
            _informationProvider.Verify(x => x.ImportController(randomModuleName, randomControllerId));
        }

        [Test]
        public void OnViewModelSubscribedToRandomController_ShouldUnsubscribeFromRandomController()
        {
            string randomModuleName = CreateRandom.String();
            int randomControllerId = CreateRandom.Int();
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel(moduleName: randomModuleName);
            viewModel.SelectedItem = MasterDetailViewModel(randomModuleName, randomControllerId);
            viewModel.SelectedItem = MasterDetailViewModel(randomModuleName, 0);
            _informationProvider.Verify(x => x.UnsubscribeFromControllerChanges(randomModuleName, randomControllerId, It.IsAny<Action<Controller>>()));
        }

        [Test]
        public void ShouldNotifyPropertyChanged()
        {
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel();
            viewModel.MonitorEvents();
            viewModel.SelectedItem = MasterDetailViewModel("AnyModule", 0);
            viewModel.ShouldRaisePropertyChangeFor(x => x.SelectedItem);
        }
    }
}
