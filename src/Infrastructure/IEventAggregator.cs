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
using System.ComponentModel.Composition;

namespace VP.FF.PT.Common.Infrastructure
{
    /// <summary>
    /// An Event Aggregator acts as a single source of events for many objects. 
    /// It registers for all the events of the many objects allowing clients to register with just the aggregator.
    /// Therefore it supports a modular approach by decouple the modules using this mediator.
    /// </summary>
    /// <remarks>
    /// This Event Aggregator implementation is based on Microsoft Reactive Extensions, NuGet assembly are required.
    /// </remarks>
    [InheritedExport]
    public interface IEventAggregator
    {
        /// <summary>
        /// Gets or creates the event.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <returns>The observable event to subscribe for.</returns>
        IObservable<TEvent> GetEvent<TEvent>();

        /// <summary>
        /// Publishes the specified event to all subscribers.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="eventObject">The event object.</param>
        void Publish<TEvent>(TEvent eventObject);
    }
}
