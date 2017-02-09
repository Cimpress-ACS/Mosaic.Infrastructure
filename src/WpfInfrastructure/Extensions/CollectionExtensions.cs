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
using System.Collections.ObjectModel;
using System.Linq;

namespace VP.FF.PT.Common.WpfInfrastructure.Extensions
{
    /// <summary>
    /// Extends the <see cref="System.Collections.ObjectModel.Collection&lt;T&gt;" /> class.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Removes the given items in a <see cref="System.Collections.ObjectModel.Collection&lt;T&gt;" />.
        /// </summary>
        /// <param name="collection">The collection of items.</param>
        /// <param name="itemsToRemove">The specific items to remove.</param>
        /// <typeparam name="T">The type of the items.</typeparam>
        public static void RemoveItems<T>(this Collection<T> collection, IEnumerable<T> itemsToRemove)
        {
            var listOfItemsToRemove = itemsToRemove.ToList();

            foreach (var itemToRemove in listOfItemsToRemove)
            {
                collection.Remove(itemToRemove);
            }
        }
    }
}
