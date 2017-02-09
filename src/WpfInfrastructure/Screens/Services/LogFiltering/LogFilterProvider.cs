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
using System.ComponentModel.Composition;
using System.Linq;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Services.LogFiltering
{
    /// <summary>
    /// The <see cref="LogFilterProvider"/> provides the log filters for
    /// each module based on the injected <see cref="IDefineLogFilters"/> instances.
    /// </summary>
    [Export(typeof(IProvideLogFilters))]
    public class LogFilterProvider : IProvideLogFilters
    {
        private readonly IEnumerable<IDefineLogFilters> _logFilterRules;

        /// <summary>
        /// Initilizes a new <see cref="LogFilterProvider"/> instance.
        /// </summary>
        /// <param name="logFilterRules">An <see cref="IEnumerable{T}"/> of <see cref="IDefineLogFilters"/> providing log filters.</param>
        [ImportingConstructor]
        public LogFilterProvider(
            [ImportMany] IEnumerable<IDefineLogFilters> logFilterRules)
        {
            _logFilterRules = logFilterRules;
        }

        /// <summary>
        /// Gets the log filter for the module with the specified <paramref name="moduleName"/>.
        /// </summary>
        /// <param name="moduleName">The name of the module whiches rules are desired.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of strings.</returns>
        public IEnumerable<string> GetLogFiltersForModule(string moduleName)
        {
            return _logFilterRules.SelectMany(r => r.GetLogFilters(moduleName));
        }
    }
}
