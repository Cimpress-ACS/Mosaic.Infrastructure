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
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Services;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Services.LogFiltering;
using IProvideLogMessages = VP.FF.PT.Common.WpfInfrastructure.Screens.Services.IProvideLogMessages;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    /// <summary>
    /// The <see cref="AlarmSummaryViewModel"/> exposes log and alarm information to its 
    /// corresponding view.
    /// </summary>
    public class AlarmSummaryViewModel : PropertyChangedBase
    {
        private readonly ICollection<string> _moduleNames;
        private readonly IResetAlarms _alarmsReseter;
        private readonly IProvideAlarms _provideAlarms;
        private readonly IProvideLogMessages _provideLogMessages;
        private readonly IProvideLogFilters _provideLogFilters;
        private readonly BindableCollection<string> _logMessages;
        private readonly ILogger _logger;
        private ObservableCollection<Alarm> _currentAlarmEntries = new ObservableCollection<Alarm>();
        private ObservableCollection<Alarm> _historicAlarmEntries = new ObservableCollection<Alarm>();

        public event Action<ObservableCollection<Alarm>> CurrentAlarmsChanged;
        public event Action<ObservableCollection<Alarm>> HistoricAlarmsChanged;

        /// <summary>
        /// Initializes a new <see cref="AlarmSummaryViewModel"/> instance.
        /// </summary>
        public AlarmSummaryViewModel()
            : this(
            string.Empty, 
            new ResetAlarmsNullObject(), 
            new AlarmsProviderNullObject(),
            new LogMessagesProviderNullObject(), 
            new LogFilterProviderNullObject(),  
            new Log4NetLogger())
        {
        }

        /// <summary>
        /// Initializes a new <see cref="AlarmSummaryViewModel"/> instance.
        /// </summary>
        /// <param name="moduleName">The name of the module this view model is dedicated to.</param>
        /// <param name="alarmsReseter">An instance capable of reseting alarms.</param>
        /// <param name="provideAlarms">An instance capable of getting alarms.</param>
        /// <param name="provideLogMessages">An instance capable of getting log messages.</param>
        /// <param name="provideLogFilters">An instance capable of getting the filters for this log view.</param>
        /// <param name="logger">The logger.</param>
        public AlarmSummaryViewModel(
            string moduleName,
            IResetAlarms alarmsReseter,
            IProvideAlarms provideAlarms,
            IProvideLogMessages provideLogMessages,
            IProvideLogFilters provideLogFilters,
            ILogger logger)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentException("null or empty not allowed for moduleName", "moduleName");

            _moduleNames = new Collection<string>();
            _moduleNames.Add(moduleName);
            _alarmsReseter = alarmsReseter;
            _provideAlarms = provideAlarms;
            _provideLogMessages = provideLogMessages;
            _provideLogFilters = provideLogFilters;
            _logger = logger;
            _logMessages = new BindableCollection<string>();
            CurrentAlarmsChanged += o => { };
            HistoricAlarmsChanged += o => { };
        }

        /// <summary>
        /// Initializes a new <see cref="AlarmSummaryViewModel"/> instance.
        /// </summary>
        /// <param name="moduleNames">A list of of the modules this view model is dedicated to.</param>
        /// <param name="alarmsReseter">An instance capable of reseting alarms.</param>
        /// <param name="provideAlarms">An instance capable of getting alarms.</param>
        /// <param name="provideLogMessages">An instance capable of getting log messages.</param>
        /// <param name="provideLogFilters">An instance capable of getting the filters for this log view.</param>
        /// <param name="logger">The logger.</param>
        public AlarmSummaryViewModel(
            ICollection<string> moduleNames,
            IResetAlarms alarmsReseter,
            IProvideAlarms provideAlarms,
            IProvideLogMessages provideLogMessages,
            IProvideLogFilters provideLogFilters,
            ILogger logger)
        {
            if (moduleNames.IsNullOrEmpty())
                throw new ArgumentException("null or empty not allowed for moduleNames", "moduleNames");

            _moduleNames = moduleNames;
            _alarmsReseter = alarmsReseter;
            _provideAlarms = provideAlarms;
            _provideLogMessages = provideLogMessages;
            _provideLogFilters = provideLogFilters;
            _logger = logger;
            _logMessages = new BindableCollection<string>();
            CurrentAlarmsChanged += o => { };
            HistoricAlarmsChanged += o => { };
        }

        /// <summary>
        /// Determines if this alarm summary has current alarms with type <see cref="AlarmType.Error"/>.
        /// </summary>
        public bool HasErrors
        {
            get { return CurrentAlarmEntries.Any(currentAlarm => currentAlarm.Type == AlarmType.Error); }
        }

        /// <summary>
        /// Determines if this alarm summary has current alarms with type <see cref="AlarmType.Warning"/>.
        /// </summary>
        public bool HasWarnings
        {
            get { return CurrentAlarmEntries.Any(currentAlarm => currentAlarm.Type == AlarmType.Warning); }
        }

        /// <summary>
        /// Gets the log messages exposed by this instance.
        /// </summary>
        public BindableCollection<string> LogMessages
        {
            get { return _logMessages; }
        }

        /// <summary>
        /// Gets or sets the current alarm entries.
        /// </summary>
        public ObservableCollection<Alarm> CurrentAlarmEntries
        {
            get { return _currentAlarmEntries; }

            set
            {
                if (_currentAlarmEntries != value)
                {
                    _currentAlarmEntries = value;
                    NotifyOfPropertyChange(() => CurrentAlarmEntries);
                }
            }
        }

        /// <summary>
        /// Gets or sets the historic alarm entries.
        /// </summary>
        public ObservableCollection<Alarm> HistoricAlarmEntries
        {
            get { return _historicAlarmEntries; }

            set
            {
                if (_historicAlarmEntries != value)
                {
                    _historicAlarmEntries = value;
                    NotifyOfPropertyChange(() => HistoricAlarmEntries);
                }
            }
        }

        /// <summary>
        /// Activates this view model. This method should get called, when the ui framework
        /// begins to display this view model.
        /// </summary>
        public async Task Activate()
        {
            _logMessages.Clear();

            try
            {
                await _provideAlarms.SubscribeForAlarmChanges(_moduleNames, UpdateAlarms);
                await _provideAlarms.RequestAlarms(_moduleNames);

                var logFilters = new List<string>();

                foreach (var moduleName in _moduleNames)
                {
                    logFilters.AddRange(_provideLogFilters.GetLogFiltersForModule(moduleName));
                }

                IEnumerable<string> messages = await _provideLogMessages.GetMessages(logFilters);

                _logMessages.AddRange(messages);
            }
            catch (Exception exception)
            {
                _logger.Warn("Activating the AlarmSummaryViewModel of the platform module(s) did throw an exception.", exception);
            }              
        }

        /// <summary>
        /// Accepts the current alarms.
        /// </summary>
        public async void AcceptAlerts()
        {
            foreach (var moduleName in _moduleNames)
            {
                try
                {
                    await _alarmsReseter.ResetAlarms(moduleName);
                }
                catch (Exception exception)
                {
                    _logger.Warn(string.Format(
                        "Accepting the current alarms of the platform module '{0}' did throw an exception.", 
                        moduleName), exception);
                }
            }
        }

        /// <summary>
        /// Deactivates this view model. This method should get called, when the ui framework
        /// begins to display another view model.
        /// </summary>
        public async Task Deactivate()
        {
                try
                {
                    await _provideAlarms.UnsubscribeFromAlarmChanges(_moduleNames, UpdateAlarms);
                }
                catch (Exception exception)
                {
                    _logger.Warn(
                        "Deactivating the AlarmSummaryViewModel of the platform module(s) did throw an exception.", exception);
                }
        }

        /// <summary>
        /// Updates the <see cref="CurrentAlarmEntries"/> and the <see cref="HistoricAlarmEntries"/> with the specified 
        /// <paramref name="updatedCurrentAlarms"/> and <paramref name="updatedHistoricAlarms"/>.
        /// </summary>
        /// <param name="updatedCurrentAlarms">An <see cref="IEnumerable{T}"/> of <see cref="Alarm"/> instances.</param>
        /// <param name="updatedHistoricAlarms">An <see cref="IEnumerable{T}"/> of <see cref="Alarm"/> instances.</param>
        public void UpdateAlarms(IEnumerable<Alarm> updatedCurrentAlarms, IEnumerable<Alarm> updatedHistoricAlarms)
        {
            UpdateCollectionWith(_currentAlarmEntries, updatedCurrentAlarms);
            UpdateCollectionWith(_historicAlarmEntries, updatedHistoricAlarms);
            NotifyOfPropertyChange(() => CurrentAlarmEntries);
            NotifyOfPropertyChange(() => HistoricAlarmEntries);
            CurrentAlarmsChanged(_currentAlarmEntries);
            HistoricAlarmsChanged(_historicAlarmEntries);
        }

        private void UpdateCollectionWith(ICollection<Alarm> collectionToUpdate, IEnumerable<Alarm> values)
        {
            collectionToUpdate.Clear();
            foreach (Alarm value in values)
                collectionToUpdate.Add(value);
        }
    }
}
