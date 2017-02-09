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

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Services.LogFiltering
{
    /// <summary>
    /// The implementer of <see cref="IProvideLogFilters"/> is capabable
    /// of getting the log filters of a module.
    /// </summary>
    public interface IProvideLogFilters
    {
        /// <summary>
        /// Gets the log filter for the module with the specified <paramref name="moduleName"/>.
        /// </summary>
        /// <param name="moduleName">The name of the module whiches rules are desired.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of strings.</returns>
        IEnumerable<string> GetLogFiltersForModule(string moduleName);
    }
}
