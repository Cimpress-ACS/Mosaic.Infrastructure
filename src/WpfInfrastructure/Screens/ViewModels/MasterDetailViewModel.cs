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
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Microsoft.Expression.Interactivity.Core;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.WpfInfrastructure.ScreenActivation;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Services;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    /// <summary>
    /// The <see cref="MasterDetailViewModel"/> provides information and interaction possibilities
    /// with a controller instance.
    /// </summary>
    public class MasterDetailViewModel : PropertyChangedBase
    {
        private const string PlcStatePrefix = "cSTA_";

        private readonly string _moduleName;
        private string _displayName;
        private readonly ICommandControllers _controllerCommander;
        private readonly IProvideStatesForScreenActivation _states;
        private readonly BindableCollection<KeyValueChange> _changes;
        private readonly BindableCollection<KeyValueUnitViewModel> _actualValues;
        private readonly BindableCollection<KeyValueUnitViewModel> _configurations;
        private readonly BindableCollection<ForcingKeyValueUnitViewModel> _inputs;
        private readonly BindableCollection<ForcingKeyValueUnitViewModel> _outputs;
        private readonly BindableCollection<KeyValueUnitViewModel> _parameters;
        private readonly ICommand _command;
        private readonly ILogger _logger;

        private BindableCollection<MasterDetailViewModel> _childs;
        private Controller _controller;
        private bool _suppressChangeEvent;
        private bool _isEngineer;

        /// <summary>
        /// Initializes a new <see cref="MasterDetailViewModel"/> instance.
        /// </summary>
        public MasterDetailViewModel()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="MasterDetailViewModel"/> instance.
        /// </summary>
        /// <param name="model">The model providing the information about the controller.</param>
        /// <param name="moduleName">The name of the module the controller belongs to.</param>
        /// <param name="controllerCommander">The controller commander to interact with the controller.</param>
        /// <param name="states">The states of this view providing loading-content-error states.</param>
        public MasterDetailViewModel(
            Controller model,
            string moduleName,
            ICommandControllers controllerCommander,
            IProvideStatesForScreenActivation states)
        {
            _controller = model;
            _moduleName = moduleName;
            _controllerCommander = controllerCommander;
            _states = states;
            _changes = new BindableCollection<KeyValueChange>();
            _actualValues = new BindableCollection<KeyValueUnitViewModel>();
            _childs = new BindableCollection<MasterDetailViewModel>();
            _command = new ActionCommand(ExecuteCommandOnController);
            _configurations = new BindableCollection<KeyValueUnitViewModel>();
            _inputs = new BindableCollection<ForcingKeyValueUnitViewModel>();
            //TODO: Rolf, please check this
            _inputs.CollectionChanged += InputsChanged;
            _outputs = new BindableCollection<ForcingKeyValueUnitViewModel>();
            //TODO: Rolf, please check this
            _outputs.CollectionChanged += OutputsChanged;
            _parameters = new BindableCollection<KeyValueUnitViewModel>();
            _logger = new Log4NetLogger();
            _logger.Init(typeof(MasterDetailViewModel));
        }

        private void InputsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ForcingKeyValueUnitViewModel item in e.OldItems)
                {
                    item.PropertyChanged -= ItemInInputsChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ForcingKeyValueUnitViewModel item in e.NewItems)
                {
                    item.PropertyChanged += ItemInInputsChanged;
                }
            }
        }

        private void OutputsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ForcingKeyValueUnitViewModel item in e.OldItems)
                {
                    item.PropertyChanged -= ItemInOutputsChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ForcingKeyValueUnitViewModel item in e.NewItems)
                {
                    item.PropertyChanged += ItemInOutputsChanged;
                }
            }
        }

        public bool IsEngineer
        {
            get
            {
                return _isEngineer;
            }
            set
            {
                if (_isEngineer != value)
                {
                    _isEngineer = value;
                    NotifyOfPropertyChange(() => IsEngineer);
                }
            }
        }

        private void ItemInInputsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => Inputs);
        }

        private void ItemInOutputsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyOfPropertyChange(() => Outputs);
        }

        public int Id
        {
            get { return _controller.Id; }
        }

        public string Name
        {
            get { return _controller.Name; }
        }

        public string PlcControllerPath
        {
            get { return _controller.PlcControllerPath; }
        }

        public string FullName
        {
            get { return _controller.FullName; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    NotifyOfPropertyChange(() => DisplayName);
                }
            }
        }

        public Controller.Mode ControllerMode
        {
            get { return _controller.ControllerMode; }
        }

        public string Type
        {
            get { return _controller.Type; }
        }

        public bool IsAutoMode
        {
            get { return ControllerMode == Controller.Mode.Auto; }
        }

        public bool IsEnabled
        {
            get { return _controller.IsEnabled; }
        }

        public bool IsSimulation
        {
            get { return _controller.IsSimulation; }
        }

        public IEnumerable<KeyValueChange> Changes
        {
            get { return _changes; }
        }

        public BindableCollection<MasterDetailViewModel> Childs
        {
            get { return _childs; }

            set
            {
                if (_childs != value)
                {
                    _childs = value;
                    NotifyOfPropertyChange(() => Childs);
                }
            }
        }

        public ControllerState ControllerState
        {
            get { return _controller.ControllerState; }
        }

        public string CurrentState
        {
            get
            {
                string currentState = _controller.CurrentState;
                if (currentState != null && currentState.StartsWith(PlcStatePrefix))
                    return currentState.TrimStart(PlcStatePrefix.ToCharArray());
                return currentState;
            }
        }

        public string CurrentSubState
        {
            get
            {
                string currentSubState = _controller.CurrentSubState;
                if (currentSubState != null && currentSubState.StartsWith(PlcStatePrefix))
                    return currentSubState.TrimStart(PlcStatePrefix.ToCharArray());
                return currentSubState;
            }
        }

        public bool EnableForcing
        {
            get { return _controller.EnableForcing; }
        }

        public string ActiveAlarm
        {
            get { return _controller.ActiveAlarm; }
        }

        public ICommand Command
        {
            get { return _command; }
        }

        public ObservableCollection<CommandViewModel> Commands
        {
            get { return _controller.Commands; }
        }

        public BindableCollection<KeyValueUnitViewModel> Parameters
        {
            get { return _parameters; }
        }

        public BindableCollection<KeyValueUnitViewModel> Configuration
        {
            get { return _configurations; }
        }

        public BindableCollection<KeyValueUnitViewModel> ActualValues
        {
            get { return _actualValues; }
        }

        public BindableCollection<ForcingKeyValueUnitViewModel> Inputs
        {
            get { return _inputs; }
        }

        public BindableCollection<ForcingKeyValueUnitViewModel> Outputs
        {
            get { return _outputs; }
        }

        /// <summary>
        /// Gets a value indicating if this instance has a controller
        /// in the specified <paramref name="questionedMode"/> as a child or model.
        /// </summary>
        /// <param name="questionedMode">The questioned mode.</param>
        /// <returns>true if this instance has a controller in the desired mode, false if not.</returns>
        public virtual bool HasControllerInMode(Controller.Mode questionedMode)
        {
            if (_controller.ControllerMode == questionedMode)
                return true;
            return _childs.Any(childViewModel => childViewModel.HasControllerInMode(questionedMode));
        }

        /// <summary>
        /// Changes the controller mode of this controller.
        /// </summary>
        public void ChangeMode()
        {
            SafeExecute(() =>
            {
                if (IsAutoMode)
                    return _controllerCommander.SetControllerToMode(_moduleName, Id, Controller.Mode.Manual);
                return _controllerCommander.SetControllerToMode(_moduleName, Id, Controller.Mode.Auto);
            });
        }

        /// <summary>
        /// Changes the simulation mode of this controller.
        /// </summary>
        public void ChangeSimulationMode(bool propagate)
        {
            SafeExecute(() =>
            {
                if (IsSimulation)
                    return _controllerCommander.DeactivateSimulation(_moduleName, Id, propagate);
                return _controllerCommander.ActivateSimulation(_moduleName, Id, propagate);
            });
        }

        /// <summary>
        /// The KeyValueChanged method will be called by KeyValueUnitViewModels which can change values.
        /// Value changes are only captured if the SuppressChangeEvent flag is not set.
        /// </summary>
        /// <param name="change">The KeyValueChange from the KeyValueUnitViewModel.</param>
        public void KeyValueChanged(KeyValueChange change)
        {
            if (SuppressChangeEvent || change == null)
                return;
            KeyValueChange previousChangeWithKey = _changes.FirstOrDefault(c => string.Equals(c.Key, change.Key));
            if (previousChangeWithKey != null)
                _changes.Remove(previousChangeWithKey);
            _changes.Add(change);
        }

        public void Reset()
        {
            SuppressChangeEvent = true;
            foreach (KeyValueUnitViewModel changedViewModels in Parameters.Concat(Configuration))
                changedViewModels.Reset();
            _changes.Clear();
            SuppressChangeEvent = false;
        }

        /// <summary>
        /// Saves the chagnes on the controller.
        /// </summary>
        public void Save()
        {
            IReadOnlyCollection<KeyValueChange> changes = _changes.MakeReadOnly();
            if (changes.Any())
                SafeExecute(() => _controllerCommander.SaveChanges(_moduleName, Id, _changes));
            Reset();
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SuppressChangeEvent flag is set.
        /// Set this flag if you want so init or reload the ViewModel. This is useful for
        /// distinguishing between user input changes of the KeyValueUnitViewModel or non-user
        /// changes by e.g. initializing the ViewModel.
        /// </summary>
        public bool SuppressChangeEvent
        {
            get { return _suppressChangeEvent; }

            set
            {
                if (_suppressChangeEvent != value)
                {
                    _suppressChangeEvent = value;
                    NotifyOfPropertyChange(() => SuppressChangeEvent);
                }
            }
        }

        public virtual void Update(Controller controller)
        {
            if (string.Equals(Id, controller.Id))
            {
                _logger.Debug(string.Format("Updating controller with id '{0}'", controller.Id));
                _controller = controller;
                UpdateKeyValueViewModelsWithTags(_actualValues, _controller.ActualValues.MakeReadOnly());
                UpdateKeyValueViewModelsWithTags(_configurations, _controller.Configurations.MakeReadOnly());
                //UpdateKeyValueViewModelsWithTags(_inputs, _controller.Inputs.MakeReadOnly());
                //UpdateKeyValueViewModelsWithTags(_outputs, _controller.Outputs.MakeReadOnly());
                UpdateKeyValueViewModelsWithTags(_parameters, _controller.Parameters.MakeReadOnly());
                foreach (Controller child in controller.Children)
                    Update(child);
                NotifyOfPropertyChange(string.Empty);
                _logger.Debug(string.Format("Finished update of controller with id '{0}'", controller.Id));
            }
            else
                foreach (MasterDetailViewModel child in Childs)
                    child.Update(controller);
        }

        private void UpdateKeyValueViewModelsWithTags(BindableCollection<KeyValueUnitViewModel> viewModels, IReadOnlyCollection<Tag> tags)
        {
            AddNotExistingToChildren(viewModels, tags);
            foreach (KeyValueUnitViewModel viewModel in viewModels)
            {
                Tag tag = tags.FirstOrDefault(t => string.Equals(t.Name, viewModel.Name));
                viewModel.Update(tag);
            }
        }

        private void AddNotExistingToChildren(BindableCollection<KeyValueUnitViewModel> viewModels, IReadOnlyCollection<Tag> newTags)
        {
            foreach (Tag newTag in newTags)
                if (viewModels.All(c => !string.Equals(c.Name, newTag.Name)))
                    viewModels.Add(new KeyValueUnitViewModel(this, newTag));
        }

        private void ExecuteCommandOnController(object parameter)
        {
            var commandName = parameter as string;
            if (commandName == null)
                return;
            SafeExecute(() => _controllerCommander.ExecuteCommand(_moduleName, Id, commandName));
        }

        private async void SafeExecute(Func<Task> actionToExecute)
        {
            try
            {
                await actionToExecute();
            }
            catch (Exception exception)
            {
                _states.ChangeToErrorState(exception.ToString());
            }
        }


    }
}
