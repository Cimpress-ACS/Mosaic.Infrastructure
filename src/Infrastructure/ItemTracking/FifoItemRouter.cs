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
using System.Linq;

namespace VP.FF.PT.Common.Infrastructure.ItemTracking
{
    public class FifoItemRouter<T> : IFifoItemRouter<T>
    {
        protected class ItemWithPort
        {
            public ItemWithPort(T item, int outputPort)
            {
                Item = item;
                OutputPort = outputPort;
            }

            public T Item { get; private set; }
            public int OutputPort { get; private set; }
        }

        protected IList<ItemWithPort> Items { get; private set; }

        public FifoItemRouter()
        {
            Items = new List<ItemWithPort>();
        }

        public bool Cancel(T item)
        {
            return Remove(item);
        }

        public void Route(T item, int outputPortIndex)
        {
            lock (_syncLock)
            {
                Items.Add(new ItemWithPort(item, outputPortIndex));
            }
        }

        private readonly object _syncLock = new object();

        public bool Remove(T item)
        {
            lock (_syncLock)
            {
                var itemToRemove = Items.FirstOrDefault(i => i.Item.Equals(item));
                if (itemToRemove != null)
                {
                    return Items.Remove(itemToRemove);
                }
                return false;
            }
        }

        public IEnumerable<T> RemoveUntil(T item, bool onlyIfExits = true)
        {
            lock (_syncLock)
            {
                if (onlyIfExits && !Items.Any(i => i.Item.Equals(item)))
                {
                    yield break;
                }

                while (Items.Count > 0 && !item.Equals(Items[0].Item))
                {
                    var first = Items[0];
                    Items.RemoveAt(0);
                    yield return first.Item;
                }
                if (Remove(item))
                {
                    yield return item;
                }
            }
        }
    }
}
