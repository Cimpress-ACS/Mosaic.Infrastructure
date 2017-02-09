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
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VP.FF.PT.Common.TestInfrastructure;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.UnitTests.Screens.ViewModels
{
    [TestFixture]
    public class MasterDetailViewModel_Save : MasterDetailViewModelTests_Base
    {
        [Test]
        public void OnViewModelWithChange_ShouldSaveChangeOnCommander()
        {
            KeyValueChange change = RandomKeyValueChange();
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.KeyValueChanged(change);
            viewModel.Save();
            _commander.Verify(c => c.SaveChanges(_randomModule, _randomController, It.IsAny<IEnumerable<KeyValueChange>>()));
        }

        [Test]
        public void OnViewModelWithoutChanges_ShouldNotSaveChanges()
        {
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.Save();
            _commander.Verify(c => c.SaveChanges(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<KeyValueChange>>()), Times.Never);
        }

        [Test]
        public void OnViewModelWithChange_ShouldClearChanges()
        {
            KeyValueChange change = RandomKeyValueChange();
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.KeyValueChanged(change);
            viewModel.Save();
            viewModel.Changes.Should().BeEmpty();
        }

        [Test]
        public void OnCommanderThatThrows_ShouldExposeErrorMessage()
        {
            string randomError = CreateRandom.String();
            _commander.Setup(c => c.SaveChanges(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<IEnumerable<KeyValueChange>>()))
                      .Throws(new Exception(randomError));
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.KeyValueChanged(RandomKeyValueChange());
            viewModel.Save();
            _states.Verify(s => s.ChangeToErrorState(It.Is<string>(e => e.Contains(randomError))));
        }
    }
}
