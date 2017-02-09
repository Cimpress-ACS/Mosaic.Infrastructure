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
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.TestInfrastructure;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Services;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Services.LogFiltering;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.UnitTests.Screens.ViewModels
{
    [TestFixture]
    public class AlarmSummaryViewModelTests
    {
        private string _randomModule;
        private Mock<IResetAlarms> _resetAlarms;
        private Mock<IProvideAlarms> _provideAlarms;
        private Mock<WpfInfrastructure.Screens.Services.IProvideLogMessages> _provideLogMessages;
        private Mock<IProvideLogFilters> _provideLogFilters;
        private string _firstMessage;

        [SetUp]
        public void Setup()
        {
            _randomModule = CreateRandom.String();
            _resetAlarms = new Mock<IResetAlarms>();
            _provideAlarms = new Mock<IProvideAlarms>();
            _provideLogMessages = new Mock<WpfInfrastructure.Screens.Services.IProvideLogMessages>();
            _provideLogFilters = new Mock<IProvideLogFilters>();
            _firstMessage = CreateRandom.String();
        }

        [Test]
        public async void Activate_ShouldGetLogMessages()
        {
            string[] randomLogFilters = { CreateRandom.String() };
            _provideLogFilters
                .Setup(p => p.GetLogFiltersForModule(_randomModule))
                .Returns(randomLogFilters);
            AlarmSummaryViewModel viewModel = CreateViewModel();

            await viewModel.Activate();

            _provideLogMessages.Verify(p => p.GetMessages(randomLogFilters));
        }

        [Test]
        public async void Activate_ShouldRequestAlarms()
        {
            AlarmSummaryViewModel viewModel = CreateViewModel();
            await viewModel.Activate();
            _provideAlarms.Verify(p => p.RequestAlarms(new Collection<string> { _randomModule} ));
        }

        [Test]
        public async void Activate_ShouldSubscribeForAlarms()
        {
            AlarmSummaryViewModel viewModel = CreateViewModel();
            await viewModel.Activate();
            _provideAlarms.Verify(p => p.SubscribeForAlarmChanges(new Collection<string> { _randomModule } , viewModel.UpdateAlarms));
        }

        [Test]
        public async void Activate_OnViewModelGettingTwoLogMessages_ShouldExposeBoth()
        {
            string secondMessage = CreateRandom.String();
            IEnumerable<string> messages = new[] {_firstMessage, secondMessage};
            _provideLogMessages.Setup(p => p.GetMessages(It.IsAny<IEnumerable<string>>())).Returns(Task.FromResult(messages));
            AlarmSummaryViewModel viewModel = CreateViewModel();
            await viewModel.Activate();
            viewModel.LogMessages.Should().Contain(new[] {_firstMessage, secondMessage});
        }

        [Test]
        public async void Activate_OnViewModelWithMessage_ShouldReplaceMessages()
        {
            IEnumerable<string> messagesOnFirstGet = new[] {_firstMessage};
            IEnumerable<string> messagesOnSecondGet = new string[] {};
            _provideLogMessages.Setup(p => p.GetMessages(It.IsAny<IEnumerable<string>>())).Returns(Task.FromResult(messagesOnFirstGet));
            AlarmSummaryViewModel viewModel = CreateViewModel();
            await viewModel.Activate();
            _provideLogMessages.Setup(p => p.GetMessages(It.IsAny<IEnumerable<string>>())).Returns(Task.FromResult(messagesOnSecondGet));
            await viewModel.Activate();
            viewModel.LogMessages.Should().BeEmpty();
        }

        [Test]
        public async void Activate_OnViewModelGettingExceptionWhenGettingLogMessages_ShouldNotThrow()
        {
            _provideLogMessages.Setup(p => p.GetMessages(It.IsAny<IEnumerable<string>>())).Throws(new Exception());
            AlarmSummaryViewModel viewModel = CreateViewModel();
            try { await viewModel.Activate(); }
            catch (Exception exception) { Assert.Fail("Should not have thrown exception {0}.", exception); }
        }

        [Test]
        public async void Activate_OnViewModelGettingExceptionWhenRequestingAlarms_ShouldNotThrow()
        {
            _provideAlarms
                .Setup(p => p.RequestAlarms(It.IsAny<ICollection<string>>()))
                .Throws(new Exception());
            AlarmSummaryViewModel viewModel = CreateViewModel();
            try { await viewModel.Activate(); }
            catch (Exception exception) { Assert.Fail("Should not have thrown exception {0}.", exception); }
        }

        [Test]
        public async void Activate_OnViewModelGettingExceptionWhenSubscribingForAlarmChanges_ShouldNotThrow()
        {
            _provideAlarms.Setup(p => p.SubscribeForAlarmChanges(It.IsAny<ICollection<string>>(), It.IsAny<Action<IEnumerable<Alarm>, IEnumerable<Alarm>>>())).Throws(new Exception());
            AlarmSummaryViewModel viewModel = CreateViewModel();
            try { await viewModel.Activate(); }
            catch (Exception exception) { Assert.Fail("Should not have thrown exception {0}.", exception); }
        }

        [Test]
        public void AcceptAlerts_ShouldDelegateToReseter()
        {
            AlarmSummaryViewModel viewModel = CreateViewModel();
            viewModel.AcceptAlerts();
            _resetAlarms.Verify(r => r.ResetAlarms(_randomModule));
        }

        [Test]
        public void AcceptAlerts_OnThrowingReseter_ShouldNotThrow()
        {
            _resetAlarms.Setup(r => r.ResetAlarms(It.IsAny<string>())).Throws(new Exception());
            AlarmSummaryViewModel viewModel = CreateViewModel();
            viewModel.Invoking(vm => vm.AcceptAlerts()).ShouldNotThrow<Exception>();
        }

        [Test]
        public async void Deactivate_ShouldUnsubscribeFromAlarmChanges()
        {
            AlarmSummaryViewModel viewModel = CreateViewModel();
            await viewModel.Deactivate();
            _provideAlarms.Verify(p => p.UnsubscribeFromAlarmChanges(new Collection<string> { _randomModule }, viewModel.UpdateAlarms));
        }

        [Test]
        public async void Deactivate_OnViewModelGettingExceptionWhenUnsubscribingForAlarmChanges_ShouldNotThrow()
        {
            _provideAlarms
                .Setup(p => p.UnsubscribeFromAlarmChanges(
                                    It.IsAny<ICollection<string>>(),
                                    It.IsAny<Action<IEnumerable<Alarm>, IEnumerable<Alarm>>>()))
                .Throws(new Exception());
            AlarmSummaryViewModel viewModel = CreateViewModel();
            try { await viewModel.Deactivate(); }
            catch (Exception exception) { Assert.Fail("Should not have thrown exception {0}.", exception); }
        }

        [Test]
        public void UpdateAlarms_WithOneCurrentAndOneHistoricAlarm_ShouldExposeBoth()
        {
            var currentAlarm = new Alarm();
            var historicAlarm = new Alarm();
            AlarmSummaryViewModel viewModel = CreateViewModel();
            viewModel.UpdateAlarms(new[] {currentAlarm}, new[] {historicAlarm});
            viewModel.CurrentAlarmEntries.Should().Contain(currentAlarm);
            viewModel.HistoricAlarmEntries.Should().Contain(historicAlarm);
        }

        private AlarmSummaryViewModel CreateViewModel()
        {
            return new AlarmSummaryViewModel(
                _randomModule,
                _resetAlarms.Object, 
                _provideAlarms.Object, 
                _provideLogMessages.Object, 
                _provideLogFilters.Object,
                new ConsoleOutLogger());
        }
    }
}
