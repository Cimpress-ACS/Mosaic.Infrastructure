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
    public class SequentialSchedulerTests
    {
        [Test]
        public void Simple_task_runs()
        {
            // setup
            var scheduler = new SequentialScheduler();
            int count = 0;

            // execute
            var t = scheduler.Schedule(() => count++, CancellationToken.None);
            t.Wait();

            // verify
            count.Should().Be(1);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void Task_are_scheduled_sequentially(int loop)
        {
            // setup
            var scheduler = new SequentialScheduler();
            int count = 0;
            Task lastTask = Task.FromResult(true);

            // execute
            for (int i = 0; i < loop; i++)
            {
                var currentLoop = i;
                lastTask = scheduler.Schedule(() =>
                {
                    TestSequence(count, currentLoop);
                    count++;
                }, CancellationToken.None);
            }
            lastTask.Wait();

            // verify
            count.Should().Be(loop);
        }

        [TestCase(2, 1)]
        [TestCase(10, 7)]
        public void Scheduled_tasks_can_be_canceled(int loop, int cancelAfter)
        {
            // setup
            var scheduler = new SequentialScheduler();
            int count = 0;
            Task lastTask = Task.FromResult(true);
            var cancellationSource = new CancellationTokenSource();

            // execute
            for (int i = 0; i < loop; i++)
            {
                var currentLoop = i;
                lastTask = scheduler.Schedule(() =>
                {
                    TestSequence(count, currentLoop);
                    count++;
                    if (cancelAfter == count)
                    {
                        cancellationSource.Cancel();
                    }
                }, cancellationSource.Token);
            }
            Assert.Throws<OperationCanceledException>(() => lastTask.Wait(cancellationSource.Token));

            // verify
            count.Should().Be(cancelAfter);
        }

        private void TestSequence(int currentSequence, int expectedSequence)
        {
            expectedSequence.Should().Be(currentSequence);
        }
    }
}
