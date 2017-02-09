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
using bbv.Common.StateMachine;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    public enum States
    {
        On,
        Off
    }

    public enum Events
    {
        TurnOn,
        TurnOff
    }

    [TestFixture]
    public class StateMachineLoggerExtensionTests
    {
        private Mock<ILogger> _loggerMock;
        private PassiveStateMachine<States, Events> _stateMachine;

        [SetUp]
        public void SetUp()
        {
            _loggerMock = new Mock<ILogger>();
            _stateMachine = new PassiveStateMachine<States, Events>("Test");
            _stateMachine.AddExtension(new StateMachineLoggerExtension<States, Events>(_loggerMock.Object));

            _stateMachine.In(States.Off)
                .On(Events.TurnOn)
                    .Goto(States.On);
            _stateMachine.In(States.On)
                .On(Events.TurnOff)
                    .Goto(States.Off)
                    .Execute(() => { throw new NotImplementedException("test exception"); });
        }

        [Test]
        public void LogInitializeTest()
        {
            _loggerMock.Setup(logger => logger.Debug("Initialized StateMachine Test with initial state On"));

            _stateMachine.Initialize(States.On);

            _loggerMock.VerifyAll();
        }
        
        [Test]
        public void LogStateTransition()
        {
            _stateMachine.Initialize(States.Off);
            _stateMachine.Start();
            _loggerMock.Setup(logger => logger.Debug("StateMachine Test fires event TurnOn"));
            _loggerMock.Setup(logger => logger.Debug("StateMachine Test switched from Off to On"));

            _stateMachine.Fire(Events.TurnOn);

            _loggerMock.VerifyAll();
        }

        [Test]
        public void LogException()
        {
            _stateMachine.MonitorEvents();
            _stateMachine.Initialize(States.On);
            _stateMachine.Start();
            _loggerMock.Setup(logger => logger.Debug("StateMachine Test fires event TurnOff"));
            _loggerMock.Setup(logger => logger.Error("TransitionException handled in StateMachine Test in transition On->Off"));

            _stateMachine.Fire(Events.TurnOff);  // this leads to an exception

            _stateMachine.ShouldRaise("TransitionExceptionThrown");
            _loggerMock.VerifyAll();
        }
    }
}
