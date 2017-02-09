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
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [TestFixture]
    public class RetryTests
    {
        [Test]
        public void Retry_succeeds_if_action_succeeds()
        {
            // setup
            var retry = new Retry(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), 1, ex => { throw ex; });

            // execute and verify
            Assert.DoesNotThrow(() => retry.Execute(() => Task.FromResult(true)));
        }

        [Test]
        public void Retry_tries_until_message_succeeds()
        {
            // setup
            int count = 0;
            var retry = new Retry(TimeSpan.FromMilliseconds(1), TimeSpan.MaxValue, 1, ex => { count++; });

            var results = new Queue<Task<bool>>();
            results.Enqueue(Task.FromResult(false));
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetException(new Exception("foo"));
            results.Enqueue(tcs.Task);
            results.Enqueue(Task.FromResult(true));

            // execute
            retry.Execute(results.Dequeue);

            // verify
            count.Should().Be(2);
        }

        [Test]
        public void Retry_fails_if_timeout_reached()
        {
            // setup
            int count = 0;
            var retry = new Retry(TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(5), 2, e => count++);

            // execute
            var ex = Assert.Throws<AggregateException>(() => retry.Execute(() => Task.FromResult(false)));

            // verify
            count.Should().Be(3); // 1ms, 2ms, 4ms
        }

        [Test]
        public void Retry_collects_all_exceptions()
        {
            // setup
            var individualExceptions = new List<Exception>();
            var retry = new Retry(TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(5), 2, individualExceptions.Add);

            // execute
            var ex = Assert.Throws<AggregateException>(() => retry.Execute(() => { throw new Exception(); }));

            // verify
            individualExceptions.Count.Should().Be(3); // 1ms, 2ms, 4ms
            ((AggregateException)ex.InnerExceptions[0]).InnerExceptions.Should().Equal(individualExceptions);
        }

        [Test]
        public void NextIntervalCalculator_does_not_overflow()
        {
            // setup
            var c = new Retry.NextIntervalCalculator();

            // execute
            var nextInterval = c.GetNextInterval(1.5f, TimeSpan.FromTicks(long.MaxValue));

            // verify
            nextInterval.Should().Be(TimeSpan.FromTicks(long.MaxValue));
        }

        [Test]
        public void NextIntervalCalculator_calculates_next_interval_correctly()
        {
            // setup
            var c = new Retry.NextIntervalCalculator();

            // execute
            var nextInterval = c.GetNextInterval(2f, TimeSpan.FromTicks(1000));

            // verify
            nextInterval.Should().Be(TimeSpan.FromTicks(2000));
        }
    }
}
