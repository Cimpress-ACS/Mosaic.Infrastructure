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
using NUnit.Framework;
using VP.FF.PT.Common.TestInfrastructure;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.UnitTests.Screens.ViewModels
{
    [TestFixture]
    public class MasterDetailViewModelTests_KeyValueChanged : MasterDetailViewModelTests_Base
    {
        [Test]
        public void WithChange_ShouldStoreChange()
        {
            KeyValueChange change = RandomKeyValueChange();
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.KeyValueChanged(change);
            viewModel.Changes.Should().ContainSingle(c => Equals(c, change));
        }

        [Test]
        public void OnViewModelWithSuppressedChanges_ShouldNotStoreChange()
        {
            KeyValueChange change = RandomKeyValueChange();
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.SuppressChangeEvent = true;
            viewModel.KeyValueChanged(change);
            viewModel.Changes.Should().NotContain(change);
        }

        [Test]
        public void OnViewModelWithChange_WithSameChangeAgain_ShouldReplaceOldChange()
        {
            KeyValueChange change = RandomKeyValueChange();
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.KeyValueChanged(RandomKeyValueChange());
            viewModel.KeyValueChanged(change);
            viewModel.Changes.Should().ContainSingle(c => Equals(c, change));
        }

        [Test]
        public void WithNull_ShouldNotAddNullToChanges()
        {
            MasterDetailViewModel viewModel = CreateMasterDetailViewModel();
            viewModel.KeyValueChanged(null);
            viewModel.Changes.Should().NotContainNulls();
        }
    }
}
