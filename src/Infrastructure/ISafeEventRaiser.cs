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

namespace VP.FF.PT.Common.Infrastructure
{
    /// <summary>
    /// Helper which raises an event within a try-catch block and automatically unsubscribe the observer in case of error.
    /// </summary>
    /// <remarks>
    /// E.g. useful for WCF services using callbacks (=events): They must not crash when a client is not responsible anymore, they
    /// will be just unsubscribes from the event invokation list.
    /// </remarks>
    public interface ISafeEventRaiser
    {
        void Raise(ref Action @event);

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <typeparam name="T">The type of the dto</typeparam>
        /// <param name="event">The event.</param>
        /// <param name="dto">The dto.</param>
        void Raise<T>(ref Action<T> @event, T dto);

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <typeparam name="T1">The type of the 1. dto</typeparam>
        /// <typeparam name="T2">The type of the 2. dto</typeparam>
        /// <param name="event">The event.</param>
        /// <param name="dto1">The dto1.</param>
        /// <param name="dto2">The dto2.</param>
        void Raise<T1, T2>(ref Action<T1, T2> @event, T1 dto1, T2 dto2);

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <typeparam name="T1">The type of the 1. dto</typeparam>
        /// <typeparam name="T2">The type of the 2. dto</typeparam>
        /// <typeparam name="T3">The type of the 3. dto</typeparam>
        /// <param name="event">The event.</param>
        /// <param name="dto1">The dto1.</param>
        /// <param name="dto2">The dto2.</param>
        /// <param name="dto3">The dto3.</param>
        void Raise<T1, T2, T3>(ref Action<T1, T2, T3> @event, T1 dto1, T2 dto2, T3 dto3);

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <typeparam name="T1">The type of the 1. dto</typeparam>
        /// <typeparam name="T2">The type of the 2. dto</typeparam>
        /// <typeparam name="T3">The type of the 3. dto</typeparam>
        /// <typeparam name="T4">The type of the 4. dto</typeparam>
        /// <param name="event">The event.</param>
        /// <param name="dto1">The dto1.</param>
        /// <param name="dto2">The dto2.</param>
        /// <param name="dto3">The dto3.</param>
        /// <param name="dto4">The dto4.</param>
        void Raise<T1, T2, T3, T4>(ref Action<T1, T2, T3, T4> @event, T1 dto1, T2 dto2, T3 dto3, T4 dto4);

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <typeparam name="T1">The type of the 1. dto</typeparam>
        /// <typeparam name="T2">The type of the 2. dto</typeparam>
        /// <typeparam name="T3">The type of the 3. dto</typeparam>
        /// <typeparam name="T4">The type of the 4. dto</typeparam>
        /// <typeparam name="T5">The type of the 5. dto</typeparam>
        /// <param name="event">The event.</param>
        /// <param name="dto1">The dto1.</param>
        /// <param name="dto2">The dto2.</param>
        /// <param name="dto3">The dto3.</param>
        /// <param name="dto4">The dto4.</param>
        /// <param name="dto5">The dto5.</param>
        void Raise<T1, T2, T3, T4, T5>(ref Action<T1, T2, T3, T4, T5> @event, T1 dto1, T2 dto2, T3 dto3, T4 dto4, T5 dto5);
    }
}
