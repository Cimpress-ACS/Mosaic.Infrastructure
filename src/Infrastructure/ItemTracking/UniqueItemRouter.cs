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
    public class UniqueItemRouter<T> : IItemRouter<T>
    {
        /// <summary>
        /// Gets the current item routings.
        /// </summary>
        protected IDictionary<T, int> CurrentItemRoutings { get; private set; }

        public UniqueItemRouter()
        {
            CurrentItemRoutings = new Dictionary<T, int>();
        }

        public bool Cancel(T item)
        {
            return CurrentItemRoutings.Remove(item);
        }

        public void Route(T item, int outputPortIndex)
        {
            CurrentItemRoutings.Add(item, outputPortIndex);
        }
    }
}
