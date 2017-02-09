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

namespace VP.FF.PT.Common.Infrastructure.Concurrency
{
    /// <summary>
    /// Polling based action executer helper.
    /// Concurrency safe alternative to timers, use this on a single thread and call
    /// it regularly (e.g. during a PLC heartbeat poll).
    /// This is a fully synchronous solution.
    /// </summary>
    [InheritedExport]
    public interface IApplicationDispatcher
    {
        /// <summary>
        /// Call this method regularly to check for new actions to execute.
        /// </summary>
        void ExecuteInvokes();

        /// <summary>
        /// Adds an action to execute directly with the next poll call.
        /// This method respects the order when adding multiple actions.
        /// </summary>
        /// <param name="action">The action to execute as soon as possible.</param>
        void Invoke(Action action);

        /// <summary>
        /// Adds an action to execute directly with the next poll call.
        /// This method respects the order when adding multiple actions.
        /// </summary>
        /// <param name="action">The action to execute as soon as possible.</param>
        /// <param name="argument">Argument to pass to the action.</param>
        void Invoke(Action<object> action, object argument);

        /// <summary>
        /// Adds an action with specified delay time in ms.
        /// Adding nested actions inside an action is supported.
        /// </summary>
        /// <param name="delay">The delay time in milliseconds.</param>
        /// <param name="action">The action to execute.</param>
        void AddDelayedAction(int delay, Action action);

        /// <summary>
        /// Adds an action to execute after a specified number of poll calls.
        /// Adding nested actions inside an action is supported.
        /// </summary>
        /// <remarks>
        /// If are multiple actions with same poll number around the action execution order 
        /// if equals the adding order.
        /// </remarks>
        /// <param name="numberOfPolls">The number of polls.</param>
        /// <param name="action">The action.</param>
        void AddActionAfterPolls(int numberOfPolls, Action action);

        /// <summary>
        /// Cancells all planned actions.
        /// </summary>
        void CancellAll();
    }
}
