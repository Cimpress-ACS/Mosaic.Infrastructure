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

namespace VP.FF.PT.Common.Infrastructure.ItemTracking
{
    /// <summary>
    /// Extends the <see cref="IItemRouter&lt;T&gt;"/> with FIFO capabilities.
    /// </summary>
    /// <typeparam name="T">The item type that should be routed.</typeparam>
    public interface IFifoItemRouter<T> : IItemRouter<T>
    {
        /// <summary>
        /// Removes items until the given item can be found.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <param name="onlyIfExists">Flag whether to remove the items only if the item exists in the list. Setting this to false will remove all entries if the item cannot be found.</param>
        /// <returns>The list of removed items.</returns>
        IEnumerable<T> RemoveUntil(T item, bool onlyIfExists = true);
    }
}
