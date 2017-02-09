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
using System.Threading.Tasks;

namespace VP.FF.PT.Common.WpfInfrastructure.Threading
{
    /// <summary>
    /// Dispatches an action or func to the UI thread.
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Execute an action that needs to run on the UI thread.
        /// </summary>
        /// <param name="actionToDispatch">The action to execute.</param>
        void Dispatch(Action actionToDispatch);

        /// <summary>
        /// Execute a func that needs to run on the UI thread.
        /// </summary>
        /// <param name="functionToDispatch">The func to execute.</param>
        /// <returns>The task of functionToDispatch.</returns>
        Task Dispatch(Func<Task> functionToDispatch);

        /// <summary>
        /// Execute a func that needs to run on the UI thread.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="functionToDispatch">The func to execute.</param>
        /// <returns>The task of functionToDispatch.</returns>
        Task<TResult> Dispatch<TResult>(Func<Task<TResult>> functionToDispatch);
    }
}
