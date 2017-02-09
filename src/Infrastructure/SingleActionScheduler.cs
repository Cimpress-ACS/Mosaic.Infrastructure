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
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace VP.FF.PT.Common.Infrastructure
{
    public class SingleActionScheduler : IPlatformScheduler
    {
        private readonly BlockingCollection<Tuple<Task, CancellationTokenSource>> _tasks;
        private readonly CancellationTokenSource _loopToken;
        private readonly IDisposable _loop;
        private readonly object _lock = new object();

        public SingleActionScheduler()
        {
            _tasks = new BlockingCollection<Tuple<Task, CancellationTokenSource>>();
            _loopToken = new CancellationTokenSource();
            _loop = Task.Run(() =>
            {
                while (!_tasks.IsCompleted)
                {
                    var pair = _tasks.Take();
                    var task = pair.Item1;
                    try
                    {
                        task.RunSynchronously();
                    }
                    catch (Exception)
                    {
                    }
                }
            }, _loopToken.Token);
        }

        public Task Schedule(Action action)
        {
            return Schedule(action, CancellationToken.None);
        }

        public Task Schedule(Action action, CancellationToken cancellationToken)
        {
            lock (_lock)
            {
                Tuple<Task, CancellationTokenSource> pair;
                while (_tasks.TryTake(out pair))
                {
                    pair.Item2.Cancel();
                }

                // ReSharper disable once PossiblyMistakenUseOfParamsMethod
                var source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var task = new Task(action, source.Token);
                _tasks.Add(new Tuple<Task, CancellationTokenSource>(task, source), _loopToken.Token);
                return task;
            }
        }
    }
}
