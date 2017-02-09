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
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Services
{
    /// <summary>
    /// The implementer provides information about plc controllers.
    /// </summary>
    public interface IProvidePlcInformation
    {
        /// <summary>
        /// Export the current configuration of the plc controllers of the module with the specified <paramref name="moduleName"/>.
        /// </summary>
        /// <param name="moduleName">The name of the module the controllers belong to.</param>
        /// <returns>A <see cref="Task"/> instance providing a <see cref="string"/>.</returns>
        Task<string> Export(string moduleName);

        /// <summary>
        /// Imports the configuration saved in the specified <paramref name="exportedControllerTree"/> 
        /// to the plc controllers of the module with the specified <paramref name="moduleName"/>.
        /// </summary>
        /// <param name="moduleName">The name of the module the configuration should get imported to.</param>
        /// <param name="exportedControllerTree">A xml structure with controller configurations.</param>
        /// <exception cref="InvalidOperationException">
        /// The method could throw an invalid operation exception p.e. when the <paramref name="exportedControllerTree"/> does not
        /// contain an xml structure.
        /// It won't throw exception when the configuration refers to invalid values and controllers.
        /// </exception>
        /// <returns>A <see cref="Task"/> instance providing a <see cref="ImportExportResultViewModel"/>.</returns>
        Task<ImportExportResultViewModel> Import(string moduleName, string exportedControllerTree);

        /// <summary>
        /// Gets the whole controller tree of a module with all already imported values. Does not trigger a new import.
        /// </summary>
        /// <param name="moduleName">The name of the module the controllers belong to.</param>
        /// <returns>A <see cref="Task"/> instance providing the root element of the controller tree.</returns>
        Task<Controller> LoadRootController(string moduleName);

        /// <summary>
        /// Imports the controller with the specified <paramref name="controllerId"/> from the module with the specified
        /// <paramref name="moduleName"/> with all his values.
        /// </summary>
        /// <param name="moduleName">The name of the module the controller belongs to.</param>
        /// <param name="controllerId">The id of the controller to import.</param>
        /// <returns>A <see cref="Task"/> instance providing the controller.</returns>
        Task<Controller> ImportController(string moduleName, int controllerId);

        /// <summary>
        /// Sets all controllers of the module with the specified <paramref name="moduleName"/> to the specified <paramref name="mode"/>.
        /// </summary>
        /// <param name="moduleName">The name of the module the controllers belong to.</param>
        /// <param name="mode">The mode the controllers should get set to.</param>
        Task SetAllControllersToMode(string moduleName, Controller.Mode mode);

        /// <summary>
        /// Subscribes the specified <paramref name="handler"/> to get executed when ever overall information of the contoller
        /// tree on the module with the specified <paramref name="moduleName"/> changes.
        /// </summary>
        /// <param name="moduleName">The name of the module to observe.</param>
        /// <param name="handler">The handler to invoke on an update.</param>
        Task SubscribeForTreeUpdate(string moduleName, Action<Controller> handler);

        /// <summary>
        /// Unsubscribes the specified <paramref name="handler"/> from getting executed when the overall information of the controller tree
        /// on the moduel with the specified <paramref name="moduleName"/> changes.
        /// </summary>
        /// <param name="moduleName">The name of the module to observe.</param>
        /// <param name="handler">The handler to not anymore invoke on an update.</param>
        Task UnsubscribeFromTreeUpdate(string moduleName, Action<Controller> handler);

        /// <summary>
        /// Registers the specified <paramref name="handler"/> to get executed when ever the controller with the specified <paramref name="controllerId"/>
        /// on the module with the specified <paramref name="moduleName"/> changes.
        /// </summary>
        /// <param name="moduleName">The name of the module the controller belongs to.</param>
        /// <param name="controllerId">The id of the controller to monitor.</param>
        /// <param name="handler">The handler that gets executed when the controller changes.</param>
        void SubscribeForControllerChanges(string moduleName, int controllerId, Action<Controller> handler);

        /// <summary>
        /// Unregisters the specified <paramref name="handler"/> from getting executed when the controller with the specified <paramref name="controllerId"/>
        /// on the module with the specified <paramref name="moduleName"/> changes.
        /// </summary>
        /// <param name="moduleName">The name of the module the controller belongs to.</param>
        /// <param name="controllerId">The id of the controller to not monitor anymore.</param>
        /// <param name="handler">The handler that should not longer get executed when the controller changes.</param>
        void UnsubscribeFromControllerChanges(string moduleName, int controllerId, Action<Controller> handler);
    }
}
