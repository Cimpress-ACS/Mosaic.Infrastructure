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


using System.Collections.Generic;
using System.Threading.Tasks;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Services
{
    /// <summary>
    /// The implementer provides functions to command a certain controller on a module.
    /// </summary>
    public interface ICommandControllers
    {
        /// <summary>
        /// Activates simulation mode on the controller with the specified <paramref name="controllerId"/>.
        /// </summary>
        /// <param name="moduleName">The name of the module the commanded controller belongs to.</param>
        /// <param name="controllerId">The id of the controller to command.</param>
        /// <param name="propagate">True = Set all subcontrollers also to Simulation / False = Clear the simulation for all sub controllers</param>
        Task ActivateSimulation(string moduleName, int controllerId, bool propagate);
        
        /// <summary>
        /// Deactivates simulation mode on the controller with the specified <paramref name="controllerId"/>.
        /// </summary>
        /// <param name="moduleName">The name of the module the commanded controller belongs to.</param>
        /// <param name="controllerId">The id of the controller to command.</param>
        /// <param name="propagate">True = Set all subcontrollers also to Simulation / False = Clear the simulation for all sub controllers</param>
        Task DeactivateSimulation(string moduleName, int controllerId, bool propagate);

        /// <summary>
        /// Sets the controller with the specified <paramref name="controllerId"/> into the specified <paramref name="mode"/>.
        /// </summary>
        /// <param name="moduleName">The name of the module the commanded controller belongs to.</param>
        /// <param name="controllerId">The id of the controller to command.</param>
        /// <param name="mode">The mode the controller should change to.</param>
        Task SetControllerToMode(string moduleName, int controllerId, Controller.Mode mode);

        /// <summary>
        /// Executes the command with the specified <paramref name="commandName"/> on the controller
        /// with the specified <paramref name="controllerId"/>.
        /// </summary>
        /// <param name="moduleName">The name of the module the commanded controller belongs to.</param>
        /// <param name="controllerId">The id of the controller to command.</param>
        /// <param name="commandName">The name of the command to execute on the controler.</param>
        Task ExecuteCommand(string moduleName, int controllerId, string commandName);

        /// <summary>
        /// Saves the specified <paramref name="changes"/> to the controller with the specified <paramref name="controllerId"/>.
        /// </summary>
        /// <param name="moduleName">The name of the module the commanded controller belongs to.</param>
        /// <param name="controllerId">The id of the controller to command.</param>
        /// <param name="changes">An <see cref="IEnumerable{T}"/> of <see cref="KeyValueChange"/> to save on the controler.</param>
        Task SaveChanges(string moduleName, int controllerId, IEnumerable<KeyValueChange> changes);
    }
}
