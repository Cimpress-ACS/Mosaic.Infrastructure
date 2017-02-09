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
using System.Threading;
using System.Threading.Tasks;

namespace VP.FF.PT.Common.Infrastructure
{
    public class Retry
    {
        private readonly TimeSpan _startRetryInterval;
        private readonly TimeSpan _maxRetryInterval;
        private readonly float _retryIntervalIncrease;
        private readonly Action<Exception> _errorLog;
        private readonly NextIntervalCalculator _intervalCalculator;

        public Retry(TimeSpan startRetryInterval, TimeSpan maxRetryInterval, float retryIntervalIncrease = 2, Action<Exception> errorLog = null)
        {
            _startRetryInterval = startRetryInterval;
            _maxRetryInterval = maxRetryInterval;
            _retryIntervalIncrease = retryIntervalIncrease;
            _errorLog = errorLog;
            _intervalCalculator = new NextIntervalCalculator();
        }

        public void Execute(Func<Task<bool>> action)
        {
            var t = ExecuteAsync(action, CancellationToken.None);
            t.Wait();
        }

        public async Task ExecuteAsync(Func<Task<bool>> action, CancellationToken token)
        {
            var exceptions = new List<Exception>();

            var currentRetryInterval = _startRetryInterval;
            while (currentRetryInterval < _maxRetryInterval)
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    if (await action())
                    {
                        return;
                    }
                    if (_errorLog != null)
                    {
                        _errorLog(new Exception("Retry action returned false."));
                    }
                }
                catch (Exception ex)
                {
                    if (_errorLog != null)
                    {
                        _errorLog(ex);
                    }
                    exceptions.Add(ex);
                }

                await Task.Delay(currentRetryInterval, token);

                currentRetryInterval = _intervalCalculator.GetNextInterval(_retryIntervalIncrease, currentRetryInterval);
            }

            throw new AggregateException(exceptions);
        }

        internal class NextIntervalCalculator
        {
            internal TimeSpan GetNextInterval(float retryIntervalIncrease, TimeSpan currentRetryInterval)
            {
                var ticks = currentRetryInterval.Ticks * retryIntervalIncrease;
                if (ticks > long.MaxValue)
                {
                    return TimeSpan.MaxValue;
                }
                return TimeSpan.FromTicks((long) ticks);
            }
        }
    }
}
