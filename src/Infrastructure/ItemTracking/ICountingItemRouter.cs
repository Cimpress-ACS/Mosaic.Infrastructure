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


namespace VP.FF.PT.Common.Infrastructure.ItemTracking
{
    /// <summary>
    /// Extends the <see cref="IItemRouter&lt;T&gt;"/> with counting capabilities.
    /// </summary>
    /// <typeparam name="T">The item type that should be routed.</typeparam>
    public interface ICountingItemRouter<in T> : IItemRouter<T>
    {
        /// <summary>
        /// The current items that are in the router.
        /// </summary>
        /// <returns>The count of items.</returns>
        int Count();
    }
}
