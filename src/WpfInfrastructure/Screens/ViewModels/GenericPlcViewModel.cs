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
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration;
using VP.FF.PT.Common.WpfInfrastructure.ScreenActivation;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Services;
using VP.FF.PT.Common.WpfInfrastructure.Threading;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    /// <summary>
    /// The <see cref="GenericPlcViewModel"/> provides information about all controllers
    /// of a given platform module to its corresponding view: the generic page.
    /// </summary>
    public class GenericPlcViewModel : BaseViewModel, IModuleScreen
    {
        private const string GenericPlcViewIconKey = "Detail";

        //TODO: Need to implement IModuleScreen to auto-map viewmodel to view

        private readonly IProvidePlcInformation _informationProvider;
        private readonly ICommandControllers _controllerCommander;
        private readonly IProvideStatesForScreenActivation _states;
        private readonly IProvideStatesForScreenActivation _detailStates;
        private readonly ISaveToFileSystem _saver;
        private readonly ILoadFromFileSystem _loader;
        private readonly IAskUser _askUser;
        private readonly BindableCollection<MasterDetailViewModel> _masterTree;
        private readonly IModuleScreen _parenModuleScreen;
        private readonly string _moduleName;
        private readonly int _sortOrder;
        private ImportExportResultViewModel _importResultViewModel;
        private ImportExportResultViewModel _exportResultViewModel;
        private MasterDetailViewModel _selectedItem;
        private bool _isEnabled;
        private bool _isEngineer;

        /// <summary>
        /// Initializes a new <see cref="GenericPlcViewModel"/> instance.
        /// </summary>
        public GenericPlcViewModel()
        {
            _states = new ScreenActivationNullObject();
            _detailStates = new ScreenActivationNullObject();
            _masterTree = new BindableCollection<MasterDetailViewModel>();
            _sortOrder = 0;
            _importResultViewModel = new ImportExportResultNullObject();
            _exportResultViewModel = new ImportExportResultNullObject();
        }

        /// <summary>
        /// Initializes a new <see cref="GenericPlcViewModel"/> instance.
        /// </summary>
        /// <param name="parent">The parent screen.</param>
        public GenericPlcViewModel(IModuleScreen parent)
            : this()
        {
            _parenModuleScreen = parent;
            DisplayName = "Detail View";
        }

        /// <summary>
        /// Initializes a new <see cref="GenericPlcViewModel"/> instance.
        /// </summary>
        /// <param name="informationProvider">
        /// The information provider is mainly used to get information about the platform
        /// modules plc controllers, but it also provides some interaction possibilities with those controllers.
        /// </param>
        /// <param name="controllerCommander">
        /// The controller commander provides functions to interact with a certain controller.
        /// This view model is responsible for creating <see cref="MasterDetailViewModel"/> instances which make
        /// use of the controller commander.
        /// </param>
        /// <param name="states">
        /// The states represent the three main view states of the generic page (Loading - Content - Error) and
        /// provide functions to switch between those.
        /// </param>
        /// <param name="detailStates">
        /// The detail states represent the three detail view states of the generic page (Loading - Content - Error) and
        /// provide functions to switch between those.
        /// </param>
        /// <param name="saver">
        /// The saver provides the possibility to save a string to the filesystem, used by the export
        /// functionality of the generic page.
        /// </param>
        /// <param name="loader">
        /// The loader provides the possibility to load a file from the filesystem, used by the import 
        /// functionality of the generic page.
        /// </param>
        /// <param name="askUser">
        /// The ask user instance can get used to interact with the user.
        /// </param>
        /// <param name="parent">
        /// The view model of the parent screen is used to navigate back to it.
        /// </param>
        /// <param name="moduleName">
        /// The module name identifies the module this view model is responsible for.
        /// </param>
        /// <param name="dispatcher">
        /// The dispatcher is used to delegate all const-intensive operation to background threads and vice versa.
        /// </param>
        public GenericPlcViewModel(
            IProvidePlcInformation informationProvider,
            ICommandControllers controllerCommander,
            IProvideStatesForScreenActivation states,
            IProvideStatesForScreenActivation detailStates,
            ISaveToFileSystem saver,
            ILoadFromFileSystem loader,
            IAskUser askUser,
            IModuleScreen parent,
            string moduleName,
            IDispatcher dispatcher)
            : this()
        {
            _informationProvider = informationProvider;
            _controllerCommander = controllerCommander;
            _states = states;
            _detailStates = detailStates;
            _saver = saver;
            _loader = loader;
            _askUser = askUser;
            _moduleName = moduleName;
            _parenModuleScreen = parent;
            DisplayName = "Detail View";
        }

        /// <summary>
        /// Would get the detail view if it would exist, but the generic page
        /// does not have a detail view, therefor this property always returns <code>null</code>.
        /// </summary>
        public override GenericPlcViewModel DetailViewModel
        {
            get { return null; }
        }

        public string ModuleName
        {
            get { return _moduleName; }
        }

        /// <summary>
        /// Gets the icon key of the generic page.
        /// </summary>
        public override string IconKey
        {
            get { return GenericPlcViewIconKey; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this module is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        /// <summary>
        /// Indicates whether an user has engineer rights
        /// </summary>
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

        /// <summary>
        /// Gets the <see cref="ImportExportResultViewModel"/> which provides
        /// information about the last import operation that took place on the backend before.
        /// </summary>
        public ImportExportResultViewModel ImportResultViewModel
        {
            get { return _importResultViewModel; }
            private set
            {
                if (_importResultViewModel != value)
                {
                    _importResultViewModel = value;
                    NotifyOfPropertyChange(() => ImportResultViewModel);
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="ImportExportResultViewModel"/> which provides
        /// information about the last export operation that took place on the backend before.
        /// </summary>
        public ImportExportResultViewModel ExportResultViewModel
        {
            get { return _exportResultViewModel; }
            private set
            {
                if (_exportResultViewModel != value)
                {
                    _exportResultViewModel = value;
                    NotifyOfPropertyChange(() => ExportResultViewModel);
                }
            }
        }

        /// <summary>
        /// Gets the master tree which is the root controller of the platform module.
        /// </summary>
        public BindableCollection<MasterDetailViewModel> MasterTree
        {
            get { return _masterTree; }
        }

        /// <summary>
        /// Gets the parent module screen needed that could get used by a framework function to navigate back.
        /// </summary>
        public IModuleScreen ParentViewModel
        {
            get { return _parenModuleScreen; }
        }

        /// <summary>
        /// Gets or sets the selected controller.
        /// </summary>
        public MasterDetailViewModel SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (_selectedItem != value)
                {
                    if (_selectedItem != null)
                        _informationProvider.UnsubscribeFromControllerChanges(_moduleName, _selectedItem.Id, HandleControllerUpdate);
                    _selectedItem = value;
                    if (_selectedItem != null)
                        _informationProvider.SubscribeForControllerChanges(_moduleName, _selectedItem.Id, HandleControllerUpdate);
                    NotifyOfPropertyChange(() => SelectedItem);
                    if (_selectedItem != null)
                        PollSelectedController();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the modules short name (unique). This will be set by Mosaic server.
        /// </summary>
        public string ModuleKey { get; set; }

        public int ModuleTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indication the modules instance number (important if multiple modules exist).
        /// </summary>
        public int ModuleInstance { get; set; }

        /// <summary>
        /// Determines position of the Module Screen in the shell. Lower numbers appears first, higher numbers last.
        /// </summary>
        public int SortOrder
        {
            get { return _sortOrder; }
        }

        /// <summary>
        /// Gets the states provider for the whole generic page.
        /// </summary>
        public IProvideStatesForScreenActivation States
        {
            get { return _states; }
        }

        /// <summary>
        /// Gets the states provider for the detail view.
        /// </summary>
        public IProvideStatesForScreenActivation DetailStates
        {
            get { return _detailStates; }
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected async override void OnActivate()
        {
            await SafeExecute(() => _informationProvider.SubscribeForTreeUpdate(_moduleName, HandleControllerUpdate));
            await LoadRootController();
            if (MasterTree.Count > 1)
                _informationProvider.SubscribeForControllerChanges(_moduleName, _selectedItem.Id, HandleControllerUpdate);
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected async override void OnDeactivate(bool close)
        {
            if (!close && MasterTree.Any(vm => vm.HasControllerInMode(Controller.Mode.Manual)))
                if (_askUser.AskYesOrNoQuestion(context: "There are controllers in Manual mode", question: "Set all controllers to Auto mode?"))
                    await _informationProvider.SetAllControllersToMode(_moduleName, Controller.Mode.Auto);
            if (_selectedItem != null)
                _informationProvider.UnsubscribeFromControllerChanges(_moduleName, _selectedItem.Id, HandleControllerUpdate);
            await SafeExecute(() => _informationProvider.UnsubscribeFromTreeUpdate(_moduleName, HandleControllerUpdate));
        }

        public async void Export()
        {
            await SafeExecute(async () =>
            {
                string exportedTree = await _informationProvider.Export(_moduleName);
                bool result = await _saver.SaveStringToFile(exportedTree, _moduleName, ".xml");
                ExportResultViewModel = new ImportExportResultViewModel { HasFailed = !result };
                _states.ChangeToContentState();
            });
        }

        public async void Import()
        {
            await SafeExecute(async () =>
            {
                string exportedControllerTree = await _loader.LoadFromFile(_moduleName, ".xml");
                if (string.IsNullOrEmpty(exportedControllerTree))
                    return;
                ImportResultViewModel = await _informationProvider.Import(_moduleName, exportedControllerTree);
            });
        }

        private void UpdateControllerPermissions(MasterDetailViewModel controller)
        {
            controller.IsEngineer = IsEngineer;

            foreach (var controllerChild in controller.Childs)
            {
                UpdateControllerPermissions(controllerChild);
            }
        }

        public Task Initialize()
        {
            return new Task(delegate { });
        }

        /// <summary>
        /// Loads the root controller of this module.
        /// </summary>
        public async Task LoadRootController()
        {
            _states.ChangeToLoadingState();
            await SafeExecute(async () =>
            {
                var controller = await _informationProvider.LoadRootController(_moduleName);
                var viewModel = CreateMasterDetailViewModel(controller, _controllerCommander, _moduleName);
                _masterTree.Clear();
                _masterTree.Add(viewModel);
                if (SelectedItem == null)
                    SelectedItem = MasterTree.FirstOrDefault();

                UpdateControllerPermissions(_masterTree.FirstOrDefault());

                UpdateControllerDisplayName(_masterTree.FirstOrDefault());

                _states.ChangeToContentState();
            });
        }

        private void UpdateControllerDisplayName(MasterDetailViewModel controller)
        {
            controller.DisplayName = IsDeveloper ? controller.Name : controller.FullName;

            foreach (var x in controller.Childs)
            {
                UpdateControllerDisplayName(x);
            }
        }

        public bool IsDeveloper { get; set; }

        /// <summary>
        /// Resets the changes made on the specified <paramref name="controller"/>.
        /// </summary>
        /// <param name="controller">The controller which changes should get reseted.</param>
        public void ResetChangesOnController(MasterDetailViewModel controller)
        {
            controller.Reset();
        }

        /// <summary>
        /// Saves the changes made on the specified <paramref name="controller"/>.
        /// </summary>
        /// <param name="controller">The controller which changes should get saved.</param>
        public void SaveChangesOnController(MasterDetailViewModel controller)
        {
            controller.Save();
        }

        /// <summary>
        /// Changes the mode on the specified <paramref name="controller"/>.
        /// </summary>
        /// <param name="controller">The controller to change the mode on.</param>
        public void ChangeModeOnController(MasterDetailViewModel controller)
        {
            controller.ChangeMode();
        }

        /// <summary>
        /// Changes the simulation mode on the specified <paramref name="controller"/>.
        /// </summary>
        /// <param name="controller">The controller to change the mode on.</param>
        public void ChangeSimulationModeOnController(MasterDetailViewModel controller)
        {
            controller.ChangeSimulationMode(false);
        }

        /// <summary>
        /// Changes the simulation mode on the specified <paramref name="controller"/>.
        /// </summary>
        /// <param name="controller">The controller to change the mode on.</param>
        public void ChangeSimulationPropagateModeOnController(MasterDetailViewModel controller)
        {
            controller.ChangeSimulationMode(true);
        }

        public async void SetAllControllersToAutoMode()
        {
            await SafeExecute(async () =>
            {
                await _informationProvider.SetAllControllersToMode(_moduleName, Controller.Mode.Auto);
            });
        }

        public async void SetAllControllersToManualMode()
        {
            await SafeExecute(async () =>
            {
                await _informationProvider.SetAllControllersToMode(_moduleName, Controller.Mode.Manual);
            });
        }

        public Task Shutdown()
        {
            return Task.FromResult(true);
        }

        public void Dispose()
        {
        }

        private MasterDetailViewModel CreateMasterDetailViewModel(Controller controller, ICommandControllers commander, string moduleName)
        {
            if (controller == null)
                return new MasterDetailViewModel(new Controller { Id = -1 }, moduleName, commander, _detailStates);
            IEnumerable<Controller> children = controller.Children ?? new Controller[0];
            var master = new MasterDetailViewModel(controller, moduleName, commander, _detailStates)
            {
                SuppressChangeEvent = true,
                Childs = children.Where(x => x != null)
                    .Select(x => CreateMasterDetailViewModel(x, commander, moduleName))
                    .MakeBindable(),
            };
            master.SuppressChangeEvent = false;
            return master;
        }

        private async void PollSelectedController()
        {
            _detailStates.ChangeToLoadingState();
            try
            {
                Controller controller = await _informationProvider.ImportController(_moduleName, SelectedItem.Id);
                HandleControllerUpdate(controller);
                _detailStates.ChangeToContentState();
            }
            catch (Exception exception)
            {
                _detailStates.ChangeToErrorState(exception.ToString());
            }
        }

        private void HandleControllerUpdate(Controller controller)
        {
            MasterDetailViewModel root = MasterTree.FirstOrDefault();
            if (root == null)
                return;
            root.Update(controller);
        }

        private async Task SafeExecute(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception exception)
            {
                _states.ChangeToErrorState(exception.ToString());
            }
        }
    }
}
