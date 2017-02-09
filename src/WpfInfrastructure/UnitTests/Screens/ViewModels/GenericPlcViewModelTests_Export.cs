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
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.UnitTests.Screens.ViewModels
{
    [TestFixture]
    public class GenericPlcViewModelTests_Export : GenericPlcViewModelTests_Base
    {
        [Test]
        public void ShouldSaveExportedValue()
        {
            string exportedTree = CreateRandom.String();
            _informationProvider.Setup(x => x.Export(_randomModule)).Returns(Task.FromResult(exportedTree));
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel(moduleName: _randomModule);
            viewModel.Export();
            _saver.Verify(x => x.SaveStringToFile(exportedTree, _randomModule, ".xml"));
        }

        [Test]
        public void ShouldDisplayFeedbackDialog()
        {
            string exportedTree = CreateRandom.String();
            _informationProvider.Setup(x => x.Export(_randomModule)).Returns(Task.FromResult(exportedTree));
            _saver.Setup(x => x.SaveStringToFile(exportedTree, _randomModule, ".xml")).Returns(Task.FromResult(true));
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel(moduleName: _randomModule);
            viewModel.Export();
            viewModel.ExportResultViewModel.IsVisible.Should().BeTrue();
        }

        [Test]
        public void OnFailingInformationProvider_ShouldDisplayErrorMessage()
        {
            string randomErrorMessage = CreateRandom.String();
            _informationProvider.Setup(x => x.Export(It.IsAny<string>())).Throws(new Exception(randomErrorMessage));
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel();
            viewModel.Export();
            _states.Verify(s => s.ChangeToErrorState(It.Is<string>(x => x.Contains(randomErrorMessage))));
        }

        [Test]
        public void OnFailingSaver_ShouldDisplayErrorMessage()
        {
            string randomErrorMessage = CreateRandom.String();
            _saver.Setup(x => x.SaveStringToFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception(randomErrorMessage));
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel();
            viewModel.Export();
            _states.Verify(s => s.ChangeToErrorState(It.Is<string>(x => x.Contains(randomErrorMessage))));
        }
    }
}
