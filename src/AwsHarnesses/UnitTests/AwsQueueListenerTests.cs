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
using VP.FF.PT.Common.Utils.Security;

namespace Cimpress.ACS.AwsHarnesses.UnitTests
{
    [TestFixture]
    public class AwsQueueListenerTests
    {
        private AwsQueueListener<object> _testee;

        private const string AwsAccessKey = "AKcreateyourownaccesskeyBMQ";
        private static readonly string AwsSecretKey = "WTNKbFlYUmxlVzkxY205M2JtRjNjMnRsZVE9PQ==".DecodePassword();
        private const string RegionEndpoint = "eu-west-1";

        [SetUp]
        public void SetUp()
        {
            _testee = new AwsQueueListener<object>(new Mock<ILogger>().Object, new AwsUsedQueuesRepository());
        }

        [Test]
        public void WhenInitializeWithInvalidRegion_ShouldThrowException()
        {
            var action = new Action(() => _testee.Initialize(string.Empty, "_invalidregion_", string.Empty, string.Empty));

            action.ShouldThrow<AwsQueueListenerException>("region does not exist");
        }

        [Test]
        public void WhenInitializeWithInvalidEndpoint_ShouldThrowException()
        {
            var action = new Action(() => _testee.Initialize("HTTPS://SQS.DOESNOTEXIST", "us-west-1", "invalidkey", "invalidsecret"));

            action.ShouldThrow<Exception>("endpoint does not exist");
        }

        [Ignore]
        [Test]
        public void WhenInitializeWithRealAws_ShouldNotThrow()
        {
            var initialize = new Action(() => _testee.Initialize(
                "https://sqs.eu-west-1.amazonaws.com/TODO",
                RegionEndpoint,
                AwsAccessKey,
                AwsSecretKey));

            initialize.ShouldNotThrow("endpoint does not exist");

            var start = new Action(() => _testee.StartListening());
            start.ShouldNotThrow("endpoint does not exist");

            var stop = new Action(() => _testee.StopListening());
            stop.ShouldNotThrow("endpoint does not exist");
        }

        [Test]
        public void GivenNotInitialized_IsListening_ShouldBeFalse()
        {
            _testee.IsListening.Should().BeFalse("AwsQueueListener not initialized");
        }

        [Test]
        public void GivenNotInitialized_WhenStartListening_ShouldThrow()
        {
            var action = new Action(() => _testee.StartListening().Wait());

            action.ShouldThrow<AwsQueueListenerException>().WithMessage("AwsQueueListener not initialized");
        }

        [Test]
        public void GivenNotInitialized_WhenStopListening_ShouldThrow()
        {
            var action = new Action(() => _testee.StopListening());

            action.ShouldThrow<AwsQueueListenerException>().WithMessage("AwsQueueListener not initialized");
        }

        [Test]
        public void GivenNotInitialized_WhenPurge_ShouldThrow()
        {
            var action = new Action(() => _testee.Purge());

            action.ShouldThrow<AwsQueueListenerException>().WithMessage("AwsQueueListener not initialized");
        }
    }
}
