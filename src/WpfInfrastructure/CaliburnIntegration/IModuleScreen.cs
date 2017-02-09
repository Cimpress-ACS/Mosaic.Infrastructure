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
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration
{
    /// <summary>
    /// IModuleScreens will be loaded by the shell at application startup.
    /// This way the application can be extended by modules with loose coupling.
    /// </summary>
    [InheritedExport]
    public interface IModuleScreen : IScreen, IDisposable
    {
        /// <summary>
        /// Get a value indicating the modules short name (unique). This will be set by Mosaic server.
        /// </summary>
        string ModuleKey { get; set; }

        int ModuleTypeId { get; set; }

        int ModuleInstance { get; set; }

        /// <summary>
        /// Determines position of the Module Screen in the shell. Lower numbers appears first, higher numbers last.
        /// </summary>
        int SortOrder { get; }

        /// <summary>
        /// Creates WCF client and establish connection (optional).
        /// Creates other critical/potential error stuff here. Do NOT use the constructor for this.
        /// The application will continue to run, even if an exception is thrown in the initialize phase, but just the failed module disabled (and log entries).
        /// The application will crash if an exception is thrown in the constructor, so use Initialize()!
        /// </summary>
        Task Initialize();

        /// <summary>
        /// Module should unsubscribe all events and close all open connections to prepare for application shutdown.
        /// </summary>
        Task Shutdown();

        /// <summary>
        /// Gets or sets a value indicating whether this module is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsEnabled { get; set; }
    }
}
