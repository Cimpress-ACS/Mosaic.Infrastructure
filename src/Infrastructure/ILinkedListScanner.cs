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

namespace VP.FF.PT.Common.Infrastructure
{
    /// <summary>
    /// Forces sublcasses of Item to implement Equals and GetHashCode which is really needed by LinkedListScanner!
    /// </summary>
    [Serializable]
    public abstract class Item
    {
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
    }

    public interface ILinkedItem<T> where T : Item
    {
        T ItemInFront { get; set; }
        T ItemBehind { get; set; }
    }

    public interface ILinkedListScanner<T> where T : Item, ILinkedItem<T>
    {
        /// <summary>
        /// Occurs when new item has been detected.
        /// </summary>
        event EventHandler<ItemEventArgs<T>> NewItemDetectedEvent;

        /// <summary>
        /// Occurs when any item has just been detected.
        /// </summary>
        event EventHandler<ItemEventArgs<T>> BeforeItemDetectedEvent;

        /// <summary>
        /// Occurs when any item has just been detected.
        /// </summary>
        event EventHandler<ItemEventArgs<T>> ItemDetectedEvent;

        /// <summary>
        /// Occures when expected item was not scanned anymore but meant to be in the linked-list. It's propably gone.
        /// </summary>
        event EventHandler<ItemEventArgs<T>> ItemUnexpectedlyDisappearedEvent;

        /// <summary>
        /// Occures when an item was removed by on of the Remove methods.
        /// </summary>
        event EventHandler<ItemEventArgs<T>> ItemRemovedEvent;

        /// <summary>
        /// Gets the last detected item.
        /// </summary>
        T LastDetectedItem { get; }

        /// <summary>
        /// Determines whether the specified item is in the linked-list.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        bool Contains(T item);

        /// <summary>
        /// Call this method whenever an item was detected.
        /// Will raise events, handle wirings updates list and index.
        /// </summary>
        void ItemDetected(T item);

        /// <summary>
        /// Gets the items in an ordered list.
        /// </summary>
        /// <remarks>
        /// In case of linear buffer the first item is first (FIFO). The last detected item is the last item in the list.
        /// But for a circulating buffer the next expected item (depending of last recently scanned item) is the "first" item followed by items behind. Doesn't matter which item was the longest in the list.
        /// </remarks>
        IList<T> GetOrderedList();

        /// <summary>
        /// Removes the front item and releases the links.
        /// Makes only sense for linear buffers. Has no effect for circular buffers.
        /// </summary>
        void RemoveFrontItem();

        /// <summary>
        /// Removes a single item and wire up everything.
        /// </summary>
        void RemoveItem(T item);

        /// <summary>
        /// Clears all items.
        /// </summary>
        void Clear();

        /// <summary>
        /// Get the item count
        /// </summary>
        int Count { get; }
    }

    public class ItemEventArgs<T> : EventArgs
    {
        public ItemEventArgs(T item)
        {
            Item = item;
        }

        public T Item { get; private set; }
    }
}
