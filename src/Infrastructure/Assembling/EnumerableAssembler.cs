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

namespace VP.FF.PT.Common.Infrastructure.Assembling
{
    /// <summary>
    /// Assembles an <see cref="IEnumerable{T}"/> of <typeparamref name="TTo"/> instances 
    /// out of a <see cref="IEnumerable{T}"/> of <typeparamref name="TTo"/> instances.
    /// </summary>$
    public class EnumerableAssembler<TFrom, TTo> : IAssemble<IEnumerable<TFrom>, IEnumerable<TTo>>
    {
        protected readonly IAssemble<TFrom, TTo> _instanceAssembler;

        public EnumerableAssembler(IAssemble<TFrom, TTo> instanceAssembler)
        {
            _instanceAssembler = instanceAssembler;
        }

        /// <summary>
        /// Assembles a new <see cref="IEnumerable{T}"/> of <see cref="TTo"/> instances out of the specified 
        /// <paramref name="fromItems"/>.
        /// </summary>
        /// <param name="fromItems">The original enumerable.</param>
        /// <param name="assembleParameters">The assembleParameters of get delegated to the inner instance assembler.</param>
        /// <returns>The resulting enumerable.</returns>
        public virtual IEnumerable<TTo> Assemble(IEnumerable<TFrom> fromItems, dynamic assembleParameters = null)
        {
            if (fromItems == null)
                yield break;
            foreach (TFrom fromItem in fromItems)
                yield return _instanceAssembler.Assemble(fromItem, assembleParameters);
        }
    }
}
