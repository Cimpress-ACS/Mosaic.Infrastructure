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
using System.Reactive.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    public class SampleEvent
    {
        public int Status;

        public SampleEvent()
        {
        }

        public SampleEvent(int initialStatus)
        {
            Status = initialStatus;
        }
    }

    [TestFixture]
    public class EventAggregatorTests
    {
        private EventAggregator _testee;

        [SetUp]
        public void SetUp()
        {
            _testee = new EventAggregator();
        }

        [Test]
        public void SubscriptionTest()
        {
            bool eventWasRaised = false;

            _testee.GetEvent<SampleEvent>().Subscribe(t => eventWasRaised = true);
            _testee.Publish(new SampleEvent());

            eventWasRaised.Should().BeTrue();
        }
        
        [Test]
        public void UnSubscribeTest()
        {
            bool eventWasRaised = false;
            var subscription = _testee.GetEvent<SampleEvent>()
                .Subscribe(se => eventWasRaised = true);

            subscription.Dispose();
            _testee.Publish(new SampleEvent());

            eventWasRaised.Should().BeFalse();
        }

        [Test]
        public void SelectiveSubscriptionTest()
        {
            bool eventWasRaised = false;

            _testee.GetEvent<SampleEvent>()
                .Where(se => se.Status == 1)
                .Subscribe(se => eventWasRaised = true);
            _testee.Publish(new SampleEvent { Status = 1 });

            eventWasRaised.Should().BeTrue();
        }

        [Test]
        public void GivenSubscribed_WhenPublishMultipleTimes_ShouldCallSubscribeActionMultipleTimes()
        {
            int eventWasRaisedCounter = 0;

            _testee.GetEvent<SampleEvent>().Subscribe(t => eventWasRaisedCounter++);
            _testee.Publish(new SampleEvent());
            _testee.Publish(new SampleEvent(1));
            _testee.Publish(new SampleEvent(2));

            eventWasRaisedCounter.Should().Be(3, "SampleEvent was published 3 times");
        }
    }
}
