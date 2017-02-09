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
using System.Threading;
using System.Threading.Tasks;

namespace VP.FF.PT.Common.Infrastructure
{
    public class SequentialScheduler : IPlatformScheduler
    {
        private Task _currentTask;

        private readonly object _syncScheduling = new object();

        public SequentialScheduler()
        {
            _currentTask = Task.FromResult(true);
        }

        public Task Schedule(Action taskToSchedule, CancellationToken cancellationToken)
        {
            lock (_syncScheduling)
            {
                _currentTask = _currentTask.ContinueWith(t => taskToSchedule(), cancellationToken);
                return _currentTask;
            }
        }

        public Task Schedule(Action taskToSchedule)
        {
            return Schedule(taskToSchedule, CancellationToken.None);
        }
    }
}
