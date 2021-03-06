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


using FluentAssertions;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [TestFixture]
    public class LoggerTests
    {
        [Test]
        public void WhenInitializeLogger_MustHaveCorrectLoggerName()
        {
            var logger = new Log4NetLogger();
            logger.Init(GetType());

            logger.LoggerName.Should().Be(
                "VP.FF.PT.Common.Infrastructure.UnitTests.LoggerTests");
        }

        [Test]
        public void WhenEventLog_ShouldRaiseEvent()
        {
            var testee = new LoggerWithEvent();
            testee.MonitorEvents();

            testee.ErrorFormat("this is a {0}", "test");

            testee
                .ShouldRaise("LogEvent")
                .WithArgs<LogEventArgs>(a => a.Event.RenderedMessage == "this is a test");
        }
    }
}
