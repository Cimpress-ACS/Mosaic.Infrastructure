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
    public class GenericPlcViewModelTests : GenericPlcViewModelTests_Base
    {
        [Test]
        public void SetAllControllersToAutoMode_ShouldDelegateToInformationProviderWithAuto()
        {
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel(_randomModule);
            viewModel.SetAllControllersToAutoMode();
            _informationProvider.Verify(i => i.SetAllControllersToMode(_randomModule, Controller.Mode.Auto));
        }

        [Test]
        public void SetAllControllersToAutoMode_OnThrowingProvider_ShouldDisplayErrorView()
        {
            string randomError = CreateRandom.String();
            _informationProvider.Setup(p => p.SetAllControllersToMode(It.IsAny<string>(), It.IsAny<Controller.Mode>()))
                                .Throws(new InvalidOperationException(randomError));
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel();
            viewModel.SetAllControllersToAutoMode();
            _states.Verify(s => s.ChangeToErrorState(It.Is<string>(e => e.Contains(randomError))));
        }

        [Test]
        public void SetAllControllersToManualMode_ShouldDelegateToInformationProviderWithManual()
        {
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel(_randomModule);
            viewModel.SetAllControllersToManualMode();
            _informationProvider.Verify(i => i.SetAllControllersToMode(_randomModule, Controller.Mode.Manual));
        }

        [Test]
        public void SetAllControllersToManualMode_OnThrowingProvider_ShouldDisplayErrorView()
        {
            string randomError = CreateRandom.String();
            _informationProvider.Setup(p => p.SetAllControllersToMode(It.IsAny<string>(), It.IsAny<Controller.Mode>()))
                                .Throws(new InvalidOperationException(randomError));
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel();
            viewModel.SetAllControllersToManualMode();
            _states.Verify(s => s.ChangeToErrorState(It.Is<string>(e => e.Contains(randomError))));
        }
    }
}
