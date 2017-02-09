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
using System.Threading.Tasks;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Services
{
    /// <summary>
    /// An implementer of <see cref="IProvideAlarms"/> is capable
    /// of getting alarms and notifying others about them.
    /// </summary>
    public interface IProvideAlarms
    {
        /// <summary>
        /// Requests the current and the historic alarms of the specified <paramref name="modules"/>.
        /// </summary>
        /// <param name="modules">The name of the platform modules to get the alarms from.s</param>
        /// <returns>A <see cref="Task"/> instance.</returns>
        Task RequestAlarms(ICollection<string> modules);

        /// <summary>
        /// Subscribes the specified <paramref name="handler"/> to get invoked when alarms
        /// on the specified <paramref name="modules"/> change.
        /// </summary>
        /// <param name="modules">The name of the modules to observe.</param>
        /// <param name="handler">The handler to invoke on changes.</param>
        /// <returns>A <see cref="Task"/> instance.</returns>
        Task SubscribeForAlarmChanges(ICollection<string> modules, Action<IEnumerable<Alarm>, IEnumerable<Alarm>> handler);

        /// <summary>
        /// Unsubscribes the specified <paramref name="handler"/> from getting invoked when alarms
        /// on the specified <paramref name="modules"/> change.
        /// </summary>
        /// <param name="modules">The name of the modules to observe.</param>
        /// <param name="handler">The handler to not anymore invoke on changes.</param>
        /// <returns>A <see cref="Task"/> instance.</returns>
        Task UnsubscribeFromAlarmChanges(ICollection<string> modules, Action<IEnumerable<Alarm>, IEnumerable<Alarm>> handler);
    }
}
