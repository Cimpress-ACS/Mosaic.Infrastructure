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
    public class GenericPlcViewModelTests_Import : GenericPlcViewModelTests_Base
    {
        [Test]
        public void ShouldImportLoadedFile()
        {
            string randomControllerTree = CreateRandom.String();
            _loader.Setup(x => x.LoadFromFile(_randomModule, ".xml")).Returns(Task.FromResult(randomControllerTree));
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel(_randomModule);
            viewModel.Import();
            _informationProvider.Verify(x => x.Import(_randomModule, randomControllerTree));
        }

        [Test]
        public void ShouldDisplayImportFeedback()
        {
            _loader.Setup(x => x.LoadFromFile(Loa1, ".xml")).Returns(Task.FromResult("<anyresult/>"));
            _informationProvider.Setup(x => x.Import(Loa1, "<anyresult/>")).Returns(Task.FromResult(new ImportExportResultViewModel()));
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel();
            viewModel.Import();
            viewModel.ImportResultViewModel.IsVisible.Should().BeTrue();
        }

        [Test]
        public void OnLoaderReturningEmptyString_ShouldNotImport()
        {
            _loader.Setup(x => x.LoadFromFile(Loa1, It.IsAny<string>())).Returns(Task.FromResult(string.Empty));
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel();
            viewModel.Import();
            _informationProvider.Verify(x => x.Import(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void OnFailingLoader_ShouldDisplayErrorMessage()
        {
            string randomError = CreateRandom.String();
            _loader.Setup(x => x.LoadFromFile(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception(randomError));
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel();
            viewModel.Import();
            _states.Verify(x => x.ChangeToErrorState(It.Is<string>(s => s.Contains(randomError))));
        }

        [Test]
        public void OnFailingImport_ShouldDisplayErrorMessage()
        {
            string randomError = CreateRandom.String();
            _loader.Setup(x => x.LoadFromFile(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult("My controller tree."));
            _informationProvider.Setup(x => x.Import(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception(randomError));
            GenericPlcViewModel viewModel = CreateGenericPlcViewModel();
            viewModel.Import();
            _states.Verify(x => x.ChangeToErrorState(It.Is<string>(s => s.Contains(randomError))));
        }
    }
}
