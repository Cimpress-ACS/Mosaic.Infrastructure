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
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace Cimpress.ACS.AwsHarnesses.UnitTests
{
    [TestFixture]
    public class AwsSyncQueueListenerTests
    {
        private AwsSyncQueueListener<object> _testee;

        [SetUp]
        public void SetUp()
        {
            _testee = new AwsSyncQueueListener<object>(new Mock<ILogger>().Object);
        }

        [Test]
        public void GivenNotInitialized_IsListening_ShouldBeFalse()
        {
            _testee.IsListening.Should().BeFalse("AwsSyncQueueListener not initialized");
        }

        [Test]
        public void GivenNotInitialized_WhenStartListening_ShouldThrow()
        {
            var action = new Action(() => _testee.StartListening());

            action.ShouldThrow<AwsQueueListenerException>().WithMessage("AwsSyncQueueListener not initialized");
        }

        [Test]
        public void GivenNotInitialized_WhenStopListening_ShouldThrow()
        {
            var action = new Action(() => _testee.StopListening());

            action.ShouldThrow<AwsQueueListenerException>().WithMessage("AwsSyncQueueListener not initialized");
        }

        [Test]
        public void GivenNotInitialized_WhenPurge_ShouldThrow()
        {
            var action = new Action(() => _testee.Purge());

            action.ShouldThrow<AwsQueueListenerException>().WithMessage("AwsSyncQueueListener not initialized");
        }
    }
}
