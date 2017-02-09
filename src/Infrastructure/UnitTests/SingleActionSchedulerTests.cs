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
using FluentAssertions;
using NUnit.Framework;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [TestFixture]
    public class SingleActionSchedulerTests
    {
        private class TestException : Exception
        {
        }

        [Test]
        public void Simple_task_runs()
        {
            // setup
            var scheduler = new SingleActionScheduler();
            int count = 0;

            // execute
            scheduler.Schedule(() => count++).Wait();

            // verify
            count.Should().Be(1);
        }

        [Test]
        public void Cancellation_works()
        {
            // setup
            var scheduler = new SingleActionScheduler();
            var source = new CancellationTokenSource();
            var foreverTask = scheduler.Schedule(() =>
            {
                while (true)
                {
                    source.Token.ThrowIfCancellationRequested();
                    Thread.Sleep(1);
                }
            }, source.Token);
            WaitForStart(foreverTask);

            // execute
            source.Cancel();

            // verify
            Action wait = foreverTask.Wait;
            wait.ShouldThrow<OperationCanceledException>();
        }

        [Test]
        public void Actions_wait_for_earlier_actions_to_finish()
        {
            // setup
            var scheduler = new SingleActionScheduler();

            bool blocked = true;
            var blockedTask = scheduler.Schedule(() => { while (blocked) Thread.Sleep(1); });
            WaitForStart(blockedTask);

            // execute
            int count1 = 0;
            var task1 = scheduler.Schedule(() => count1++);

            // verify
            task1.Wait(50).Should().BeFalse();

            blocked = false;
            blockedTask.Wait();
            task1.Wait();
            count1.Should().Be(1);
        }

        [Test]
        public void When_multiple_actions_scheduled_old_ones_are_canceled()
        {
            // setup
            var scheduler = new SingleActionScheduler();

            bool blocked = true;
            var blockedTask = scheduler.Schedule(() => { while (blocked) Thread.Sleep(1); });
            WaitForStart(blockedTask);

            // execute
            int count1 = 0;
            var task1 = scheduler.Schedule(() => count1++);

            int count2 = 0;
            var task2 = scheduler.Schedule(() => count2++);

            // verify
            blocked = false;
            blockedTask.Wait();

            count1.Should().Be(0);
            task1.IsCanceled.Should().BeTrue();

            task2.Wait();
            count2.Should().Be(1);
        }

        [Test]
        public void Exception_propagates()
        {
            // setup
            var scheduler = new SingleActionScheduler();

            // execute
            var t = scheduler.Schedule(() => { throw new TestException(); });

            // verify
            Action wait = t.Wait;
            wait.ShouldThrow<TestException>();
        }

        [Test]
        public void After_exception_later_actions_get_run()
        {
            // setup
            var scheduler = new SingleActionScheduler();
            WaitForStart(scheduler.Schedule(() => { throw new TestException(); }));

            // execute
            int count2 = 0;
            var task2 = scheduler.Schedule(() => count2++);

            // verify
            task2.Wait();
            count2.Should().Be(1);
        }

        [Test, Timeout(5000)]
        public void After_cancel_later_actions_get_run()
        {
            // setup
            var scheduler = new SingleActionScheduler();
            var source = new CancellationTokenSource();
            WaitForStart(scheduler.Schedule(() =>
            {
                while (true)
                {
                    source.Token.ThrowIfCancellationRequested();
                    Thread.Sleep(100);
                }
            }, source.Token));

            // execute
            var task1 = scheduler.Schedule(() => new object());
            source.Cancel();

            // verify
            task1.Wait();
        }

        private static void WaitForStart(Task task)
        {
            while (task.Status == TaskStatus.Created)
            {
                Thread.Sleep(1);
            }
        }
    }
}
