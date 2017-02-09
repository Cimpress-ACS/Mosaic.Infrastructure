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
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [TestFixture]
    public class SchedulerExtensionTests
    {
        [Test]
        public void Scheduler_waits_for_completion_of_work()
        {
            var hasEntered = false;
            int i = 0;
            var resetEvent = new ManualResetEvent(false);
            var timer = Scheduler.Default.ScheduleRecurringActionWithWait(TimeSpan.FromMilliseconds(1), () =>
            {
                hasEntered.Should().BeFalse();
                hasEntered = true;
                Thread.Sleep(10);
                hasEntered = false;
                if (++i == 10)
                {
                    resetEvent.Set();
                }
                return Task.FromResult(true);
            });

            resetEvent.WaitOne();
            timer.Dispose();
            
            i.Should().Be(10);
        }

        [Test]
        public void Scheduler_waits_for_completion_of_work_with_async()
        {
            var hasEntered = false;
            int i = 0;
            var resetEvent = new ManualResetEvent(false);
            bool failed = false;
            var timer = Scheduler.Default.ScheduleRecurringActionWithWait(TimeSpan.FromTicks(1), async () =>
            {
                if (hasEntered)
                {
                    failed = true;
                }
                hasEntered = true;
                await Task.Delay(10);
                hasEntered = false;
                if (++i == 10)
                {
                    resetEvent.Set();
                }
                await Task.FromResult(true);
            });

            resetEvent.WaitOne();
            timer.Dispose();

            failed.Should().BeFalse();
            i.Should().Be(10);
        }

        [Test]
        public void Scheduler_stops_when_disposed()
        {
            bool isDisposed = false;
            var resetEvent = new ManualResetEvent(false);
            var timer = Scheduler.Default.ScheduleRecurringActionWithWait(TimeSpan.FromMilliseconds(1), () =>
            {
                if (!isDisposed)
                {
                    resetEvent.Set();
                }
                else
                {
                    isDisposed.Should().BeFalse();
                }
                return Task.FromResult(true);
            });

            timer.Dispose();
            isDisposed = true;

            Thread.Sleep(100); // sigh... how can we test that for sure instead of just hoping it did't happen the past 100ms?
        }

        [Test]
        public void Scheduler_allows_multiple_independent_instances()
        {
            var timer1Completed = false;
            var timer2Completed = false;
            
            var resetEvent1 = new ManualResetEvent(false);
            var resetEvent2 = new ManualResetEvent(false);

            var resetEvent1Completed = new ManualResetEvent(false);
            var resetEvent2Completed = new ManualResetEvent(false);

            var timer1 = Scheduler.Default.ScheduleRecurringActionWithWait(TimeSpan.FromMilliseconds(1), () =>
            {
                resetEvent1.WaitOne();
                resetEvent2.Set();
                timer1Completed = true;
                resetEvent1Completed.Set();
                return Task.FromResult(true);
            });

            var timer2 = Scheduler.Default.ScheduleRecurringActionWithWait(TimeSpan.FromMilliseconds(2), () =>
            {
                resetEvent1.Set();
                resetEvent2.WaitOne();
                timer2Completed = true;
                resetEvent2Completed.Set();
                return Task.FromResult(true);
            });

            resetEvent1.WaitOne();
            resetEvent2.WaitOne();

            timer1.Dispose();
            timer2.Dispose();

            resetEvent1Completed.WaitOne();
            resetEvent2Completed.WaitOne();

            timer1Completed.Should().BeTrue();
            timer2Completed.Should().BeTrue();
        }

        [Test]
        public void Scheduler_first_due_time_is_different()
        {
            int i = 0;
            var resetEvent = new ManualResetEvent(false);

            var scheduler = new TestScheduler();

            IDisposable timer = scheduler.ScheduleRecurringActionWithWait(TimeSpan.FromTicks(1), TimeSpan.FromTicks(10), () =>
            {
                i++;
                resetEvent.Set();
                return Task.FromResult(true);
            });

            scheduler.AdvanceBy(1);
            i.Should().Be(1);

            scheduler.AdvanceBy(9);
            i.Should().Be(1);

            scheduler.AdvanceBy(1);
            i.Should().Be(2);

            scheduler.AdvanceBy(20);
            i.Should().Be(4);

            timer.Dispose();
        }
    }
}
