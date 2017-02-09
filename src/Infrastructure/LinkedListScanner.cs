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
using System.ComponentModel.Composition;
using System.Linq;

namespace VP.FF.PT.Common.Infrastructure
{
    /// <summary>
    /// Little helper which behaves like an item scanner. It remembers past items and establish wiring.
    /// It doesn't matter if this class is been used for a linear or circulating buffer, both is supported.
    /// The class is optimized for large buffers and frequent changes.
    /// </summary>
    [Export(typeof(ILinkedListScanner<>))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LinkedListScanner<T> : ILinkedListScanner<T> where T : Item, ILinkedItem<T>
    {
        private readonly object _lock = new object();

        internal readonly Dictionary<T, T> Items;

        public LinkedListScanner()
        {
            Items = new Dictionary<T, T>();
            ItemRemovedEvent += (sender, args) => { };
            ItemUnexpectedlyDisappearedEvent += (sender, args) => { };
        }

        /// <summary>
        /// Occurs when new item has been detected.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> NewItemDetectedEvent;

        /// <summary>
        /// Occurs when any item has just been detected.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> BeforeItemDetectedEvent;

        /// <summary>
        /// Occurs when any item has just been detected.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemDetectedEvent;

        /// <summary>
        /// Occures when expected item was not scanned anymore but meant to be in the linked-list. It's propably gone.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemUnexpectedlyDisappearedEvent;

        /// <summary>
        /// Occures when an item was removed by on of the Remove methods.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemRemovedEvent;

        /// <summary>
        /// Gets the last detected item.
        /// </summary>
        public T LastDetectedItem { get; private set; }

        /// <summary>
        /// Determines whether the specified item contains item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return Items.ContainsKey(item);
        }

        /// <summary>
        /// Call this method whenever an item was detected.
        /// Will raise events, handle wirings updates list and index.
        /// </summary>
        public void ItemDetected(T item)
        {
            lock (_lock)
            {
                if (BeforeItemDetectedEvent != null)
                    BeforeItemDetectedEvent(this, new ItemEventArgs<T>(item));

                if (!Items.ContainsKey(item))
                {
                    Items.Add(item, item);
                    WireUpNewItem(item);

                    LastDetectedItem = item;

                    if (NewItemDetectedEvent != null)
                        NewItemDetectedEvent(this, new ItemEventArgs<T>(item));
                }
                else
                {
                    var originalItem = Items[item];

                    CheckMissingItem(originalItem);

                    LastDetectedItem.ItemBehind = originalItem;
                    originalItem.ItemInFront = LastDetectedItem;

                    LastDetectedItem = originalItem;
                }

                if (ItemDetectedEvent != null)
                    ItemDetectedEvent(this, new ItemEventArgs<T>(item));
            }
        }

        /// <summary>
        /// Gets the items in an ordered list.
        /// </summary>
        /// <remarks>
        /// In case of linear buffer the first item is first (FIFO). The last detected item is the last item in the list.
        /// But for a circulating buffer the next expected item (depending of last recently scanned item) is the "first" item followed by items behind. Doesn't matter which item was the longest in the list.
        /// </remarks>
        public IList<T> GetOrderedList()
        {
            lock(_lock)
            {
                if (Items.Count == 0 || LastDetectedItem == null)
                    return new List<T>();

                var firstItems = (from i in Items.Values
                                 where i.ItemInFront == null
                                 select i).ToArray();

                if (firstItems.Count() > 1)
                {
                    Console.WriteLine("Invalid linked listed detected in LinkedListScanner. Found multiple front items.");
                    return new List<T>();
                }

                // build circular buffer list
                if (!firstItems.Any())
                {
                    var list = new List<T>();

                    T currentItem = LastDetectedItem.ItemBehind;                 

                    if (currentItem == null)
                        return new List<T>();

                    list.Add(currentItem);
                    currentItem = currentItem.ItemBehind;

                    while (!currentItem.Equals(LastDetectedItem.ItemBehind))
                    {
                        list.Add(currentItem);
                        currentItem = currentItem.ItemBehind;
                    }

                    return list;
                }
                    // build linear buffer list
                else
                {
                    var list = new List<T>();
                    T currentItem = firstItems.First();

                    while (currentItem != null)
                    {
                        list.Add(currentItem);
                        currentItem = currentItem.ItemBehind;
                    }

                    return list;
                }
            }
        }

        /// <summary>
        /// Removes the front item and releases the links.
        /// Makes only sense for linear buffers. Has no effect for circular buffers.
        /// </summary>
        public void RemoveFrontItem()
        {
            lock(_lock)
            {
                var firstItems = (from i in Items.Values
                    where i.ItemInFront == null
                    select i).ToArray();

                if (firstItems.Count() > 1)
                {
                    Console.WriteLine("Invalid linked listed detected in LinkedListScanner. Found multiple front items.");
                    return;
                }

                if (firstItems.Any())
                {
                    var firstItem = firstItems.First();

                    // set new front item
                    firstItem.ItemBehind.ItemInFront = null;

                    Items.Remove(firstItem);
                    ItemRemovedEvent(this, new ItemEventArgs<T>(firstItem));
                }
            }
        }

        public void RemoveItem(T item)
        {
            lock (_lock)
            {
                if (InternalRemoveItem(item))
                    ItemRemovedEvent(this, new ItemEventArgs<T>(item));
            }
        }

        public void Clear()
        {
            lock(_lock)
            {
                foreach (var item in Items)
                {
                    item.Value.ItemInFront = null;
                    item.Value.ItemBehind = null;
                }

                Items.Clear();
                LastDetectedItem = null;
            }
        }

        public int Count
        {
            get { return Items.Count; }
        }

        /// <summary>
        /// Circulars the linked list consistency check.
        /// Might throw an exception with inconsistency reason.
        /// </summary>
        /// <exception cref="System.Exception">
        /// Reason could be a counter mismatch or wrongly linked items.
        /// </exception>
        public void CircularLinkedListConsistencyCheck()
        {
            lock(_lock)
            {
                T currentItem = LastDetectedItem.ItemBehind;
                int i = 1;

                while (!currentItem.Equals(LastDetectedItem))
                {
                    i++;
                    if (currentItem.ItemInFront == null)
                        throw new Exception("Item " + currentItem + " has no ItemInFront!");

                    if (currentItem.ItemBehind == null)
                        throw new Exception("Item " + currentItem + " has no ItemBehind!");

                    currentItem = currentItem.ItemBehind;
                }

                if (i != Items.Count)
                    throw new Exception("Counter mismatch. ItemsCount is " + Items.Count + " but counted " + i +
                                        " in double-linked-list.");
            }
        }

        private void CheckMissingItem(T item)
        {
            if (LastDetectedItem != null)
            {
                var expectedItem = LastDetectedItem.ItemBehind;

                if (expectedItem != null && !expectedItem.Equals(item))
                {
                    RemoveAllBehind(expectedItem, item);
                }
            }
        }

        private void WireUpNewItem(T newItem)
        {
            if (LastDetectedItem != null)
            {
                var itemBehind = LastDetectedItem.ItemBehind;
                LastDetectedItem.ItemBehind = newItem;
                newItem.ItemInFront = LastDetectedItem;
                newItem.ItemBehind = itemBehind;
                 
                if (itemBehind != null)
                    itemBehind.ItemInFront = newItem;
            }
        }

        private void RemoveAllBehind(T fromItemInclusive, T toItemExclusive)
        {
            var currentItem = fromItemInclusive;

            while (currentItem != null && !currentItem.Equals(toItemExclusive))
            {
                var itemBehind = currentItem.ItemBehind;
                
                if (InternalRemoveItem(currentItem))
                    ItemUnexpectedlyDisappearedEvent(this, new ItemEventArgs<T>(currentItem));

                currentItem = itemBehind;
            }
        }

        private bool InternalRemoveItem(T item)
        {
            if (!Items.ContainsKey(item))
                return false;

            Items.Remove(item);

            if (item.Equals(LastDetectedItem))
            {
                LastDetectedItem = item.ItemInFront;
            }

            if (Items.Count == 1)
            {
                var lastItem = Items.First().Value;
                lastItem.ItemInFront = null;
                lastItem.ItemBehind = null;
            }
            else
            {
                if (item.ItemInFront != null)
                    item.ItemInFront.ItemBehind = item.ItemBehind;
                if (item.ItemBehind != null)
                    item.ItemBehind.ItemInFront = item.ItemInFront;
            }

            item.ItemInFront = null;
            item.ItemBehind = null;

            return true;
        }
    }
}
