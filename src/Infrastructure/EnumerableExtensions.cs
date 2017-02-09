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
using System.Linq;

namespace VP.FF.PT.Common.Infrastructure
{
    /// <summary>
    /// The <see cref="EnumerableExtensions"/> provides
    /// some extension methods for enumerables.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Returns all elements of the specified <paramref name="items"/> besides the first one.
        /// </summary>
        /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
        /// <param name="items">The items to get the tail from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <typeparamref name="T"/> instances.</returns>
        public static IEnumerable<T> Tail<T>(this IEnumerable<T> items)
        {
            return items.Skip(1);
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
                action(element);
        }
    }
}
