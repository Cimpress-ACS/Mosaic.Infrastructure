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
    /// Provides a global application lock to execute concurrent actions.
    /// </summary>
    [Export(typeof(IGlobalLock))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class GlobalLock : IGlobalLock
    {
        private static readonly FifoSemaphore GlobalFifoSemaphore = new FifoSemaphore(1);

        /// <summary>
        /// Executes the specified action with thread safety.
        /// </summary>
        public void Execute(Action action)
        {
            GlobalFifoSemaphore.Acquire();

            try
            {
                action.Invoke();
            }

            finally
            {
                GlobalFifoSemaphore.Release();
            }
        }
    }
}
