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


namespace VP.FF.PT.Common.Infrastructure.Assembling
{
    /// <summary>
    /// An instance implementing this interface can assemble instances of type <typeparamref name="TFrom"/>
    /// to instances of type <typeparamref name="TTo"/>.
    /// </summary>
    /// <typeparam name="TFrom">The type of the orignal object.</typeparam>
    /// <typeparam name="TTo">The type of the resulting object.</typeparam>
    public interface IAssemble<in TFrom, out TTo>
    {
        /// <summary>
        /// Assembles a new <see cref="TTo"/> instance out of the specified <paramref name="fromItem"/>.
        /// </summary>
        /// <param name="fromItem">The original object.</param>
        /// <param name="assembleParameters">Optional parameters the assemble method might consider when executed.</param>
        /// <returns>The resulting object.</returns>
        TTo Assemble(TFrom fromItem, dynamic assembleParameters = null);
    }
}
