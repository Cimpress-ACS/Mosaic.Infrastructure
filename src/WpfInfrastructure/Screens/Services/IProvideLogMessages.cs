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

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Services
{
    /// <summary>
    /// The implementer of <see cref="IProvideLogMessages"/> can be used to get
    /// log messages.
    /// </summary>
    public interface IProvideLogMessages
    {
        /// <summary>
        /// Gets the log messages which were emitted by the specified <paramref name="emitters"/>.
        /// </summary>
        /// <param name="emitters">The emitters to get the log messages from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of strings.</returns>
        Task<IEnumerable<string>> GetMessages(IEnumerable<string> emitters);
    }
}
