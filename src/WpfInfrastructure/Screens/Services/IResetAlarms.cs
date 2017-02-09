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


using System.Threading.Tasks;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Services
{
    /// <summary>
    /// An implementer of <see cref="IResetAlarms"/> is capable of
    /// reseting the current alarms of a platform modules.
    /// </summary>
    public interface IResetAlarms
    {
        /// <summary>
        /// Resets the alarms of the platform module with the name <see cref="module"/>.
        /// </summary>
        /// <param name="module">The name of the module which alarms should get reseted.</param>
        /// <returns>A <see cref="Task"/> instance.</returns>
        Task ResetAlarms(string module);
    }
}
