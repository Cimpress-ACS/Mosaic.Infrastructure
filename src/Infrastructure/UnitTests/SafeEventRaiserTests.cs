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
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [TestFixture]
    public class SafeEventRaiserTests
    {
        private static Action<string> _testMessageEvent = delegate { };
        private static Action _testMessageEventWithoutArguments = delegate { };
        private string _receivedMessage;

        private ISafeEventRaiser _testee;
        private Mock<ILogger> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _testee = new SafeEventRaiser();
            _loggerMock = new Mock<ILogger>();
            _testMessageEvent += TestMessageEvent;
            _testMessageEventWithoutArguments += TestMessageEvent;
        }

        [TearDown]
        public void Teardown()
        {
            _testMessageEvent -= TestMessageEvent;
            _testMessageEvent -= BadSubscriptionMessageEvent;
            _receivedMessage = string.Empty;
        }

        [Test]
        public void TestRaiseEvent()
        {
            _testee.Raise(ref _testMessageEvent, "test");

            _receivedMessage.Should().Be("test");
        }

        [Test]
        public void TestRaiseEventWithoutArguments()
        {
            _testee.Raise(ref _testMessageEventWithoutArguments);

            _receivedMessage.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void GivenNoSubscriber_RaiseEventMustNotFail()
        {
            _testMessageEvent -= TestMessageEvent;

            Action action = () => _testee.Raise(ref _testMessageEvent, "test");

            action.ShouldNotThrow();
        }

        [Test]
        public void GivenSubscriberThrowsException_MustUnsubscribeButContinueWithOthers()
        {
            _testMessageEvent += BadSubscriptionMessageEvent;
            int invocationListCount = _testMessageEvent.GetInvocationList().Count();

            Action action = () => _testee.Raise(ref _testMessageEvent, "test");

            action.ShouldNotThrow();
            _receivedMessage.Should().Be("test");
            _testMessageEvent.GetInvocationList().Count()
                .Should().Be(invocationListCount-1, "bad subscriber must unsubscribe but not others");
        }

        [Test]
        public void GivenLogger_MustLogUnsubscribe()
        {
            _testee = new SafeEventRaiser(_loggerMock.Object);
            _testMessageEvent += BadSubscriptionMessageEvent;
            _loggerMock.
                Setup(logger => logger.Info(It.IsAny<string>()));

            _testee.Raise(ref _testMessageEvent, string.Empty);

            _loggerMock.VerifyAll();
        }

        private void TestMessageEvent(string s)
        {
            _receivedMessage = s;
        }

        private void TestMessageEvent()
        {
            _receivedMessage = "received";
        }

        private void BadSubscriptionMessageEvent(string s)
        {
            throw new Exception("testexception");
        }
    }
}
