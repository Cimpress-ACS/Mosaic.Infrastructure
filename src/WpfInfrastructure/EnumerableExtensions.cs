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
using Caliburn.Micro;

namespace VP.FF.PT.Common.WpfInfrastructure
{
    public static class EnumerableExtensions
    {
        public static BindableCollection<T> MakeBindable<T>(this IEnumerable<T> enumerable)
        {
            return new BindableCollection<T>(enumerable);
        }

        public static ObservableCollection<T> MakeObservable<T>(this IEnumerable<T> enumerable)
        {
            return new ObservableCollection<T>(enumerable);
        }

        public static IReadOnlyCollection<T> MakeReadOnly<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                enumerable = new T[] {};
            return new ReadOnlyCollection<T>(enumerable.ToList());
        }
    }
}
