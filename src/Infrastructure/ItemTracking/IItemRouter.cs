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
    /// Routes items to a dedicated output port.
    /// </summary>
    /// <typeparam name="T">The item type that should be routed.</typeparam>
    public interface IItemRouter<in T>
    {
        /// <summary>
        /// Cancels the routing for the given item.
        /// </summary>
        /// <param name="item">The item to cancel.</param>
        /// <returns>True, if the cancellation was successful, false if unsuccessfuly or the item wasn't found.</returns>
        bool Cancel(T item);

        /// <summary>
        /// Routes an item to an output port.
        /// </summary>
        /// <param name="item">The item to route.</param>
        /// <param name="outputPortIndex">The destination of the routing.</param>
        void Route(T item, int outputPortIndex);
    }
}
