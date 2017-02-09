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
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.Concurrency;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [TestFixture]
    public class ApplicationDispatcherTests
    {
        [Test]
        public void WhenAddInvoke_ShouldExecuteImmediately()
        {
            var testee = new ApplicationDispatcher();
            var actionTester = new ActionTester();

            testee.Invoke(actionTester.TestAction);
            actionTester.WasCalled.Should().BeTrue();
        }

        [Test]
        public void WhenAddNegativeDelayedAction_ShouldExecuteImmediately()
        {
            var testee = new ApplicationDispatcher();
            var actionTester = new ActionTester();

            testee.AddDelayedAction(-10, actionTester.TestAction);
            testee.ExecuteInvokes();

            actionTester.WasCalled.Should().BeTrue();
        }

        [Test]
        public void WhenAddDelayedAction_ShouldExecuteDelayed()
        {
            var testee = new ApplicationDispatcher();
            var actionTester = new ActionTester();

            testee.AddDelayedAction(20, actionTester.TestAction);
            testee.ExecuteInvokes();

            actionTester.WasCalled.Should().BeFalse();

            Thread.Sleep(25);
            testee.ExecuteInvokes();

            actionTester.WasCalled.Should().BeTrue();
        }

        [Test]
        public void WhenClearActions_ShouldExecuteNothing()
        {
            var testee = new ApplicationDispatcher();
            var actionTester = new ActionTester(); 
            
            testee.AddDelayedAction(5, actionTester.TestAction);
            testee.AddActionAfterPolls(1, actionTester.TestAction);
            
            testee.CancellAll();
            Thread.Sleep(25);
            testee.ExecuteInvokes();

            actionTester.WasCalled.Should().BeFalse();
        }

        [Test]
        public void WhenAddMultipleActionsWithDifferentDelays_ShouldExecuteInRightOrder()
        {
            var testee = new ApplicationDispatcher();
            var action1 = new ActionTester();
            var action2 = new ActionTester();

            testee.AddDelayedAction(-1, action1.TestAction);
            testee.AddDelayedAction(5, action2.TestAction);

            testee.ExecuteInvokes();
            action1.WasCalled.Should().BeTrue();
            action2.WasCalled.Should().BeFalse();

            Thread.Sleep(25);
            testee.ExecuteInvokes();
            action2.WasCalled.Should().BeTrue();
        }

        [Test]
        public void WhenAddActions_ShouldExecuteInRightOrder()
        {
            var testee = new ApplicationDispatcher();
            var actionTester = new OrderedActionTester();

            testee.AddActionAfterPolls(1, actionTester.TestAction2);
            testee.AddActionAfterPolls(1, actionTester.TestAction1);

            testee.ExecuteInvokes();

            actionTester.FirstCalledAction.Should().Be(2);
        }

        [Test]
        public void WhenAddActionWithRemainingPolls_ShouldExecuteOnRightPollCount()
        {
            var testee = new ApplicationDispatcher();
            var actionTester = new ActionTester(); 
            
            testee.AddActionAfterPolls(2, actionTester.TestAction);
            
            testee.ExecuteInvokes();
            actionTester.WasCalled.Should().BeFalse();

            testee.ExecuteInvokes();
            actionTester.WasCalled.Should().BeTrue();
        }

        [Test]
        public void WhenAddMixedActions_AfterPoll_ShouldExecuteAll()
        {
            var testee = new ApplicationDispatcher();
            
            var action1 = new ActionTester();
            var action2 = new ActionTester();
            testee.AddDelayedAction(-1, action1.TestAction);
            testee.AddActionAfterPolls(1, action2.TestAction);

            testee.ExecuteInvokes();

            action1.WasCalled.Should().BeTrue();
            action2.WasCalled.Should().BeTrue();
        }

        [Test]
        public void WhenAddMultipleActionsWithRemainingPolls_ShouldExecuteInRightOrder()
        {
            var testee = new ApplicationDispatcher();
            var actionTester = new OrderedActionTester();

            testee.AddActionAfterPolls(1, actionTester.TestAction2);
            testee.AddActionAfterPolls(1, actionTester.TestAction1);

            testee.ExecuteInvokes();

            actionTester.FirstCalledAction.Should().Be(2, "TestAction2 was added first");
        }

        [Test]
        public void WhenAddNestedAction_ShouldWork()
        {
            var testee = new ApplicationDispatcher();
            var actionTester = new ActionTester(); 
            
            testee.AddActionAfterPolls(1, () => testee.AddActionAfterPolls(1, actionTester.TestAction));

            testee.ExecuteInvokes();
            actionTester.WasCalled.Should().BeFalse();
            testee.ExecuteInvokes();
            actionTester.WasCalled.Should().BeTrue();
        }

        [Test]
        public void TestActionWithArguments()
        {
            var testee = new ApplicationDispatcher();
            
            object receivedArgument = null;

            testee.Invoke(x => receivedArgument = x, "test");
            testee.ExecuteInvokes();

            var receivedString = receivedArgument as string;
            receivedString.Should().Be("test");
        }

        [Test]
        public void GivenException_WhenInvoke_ShouldLogInnerException()
        {
            var testee = new ApplicationDispatcher();
            var actionTester = new ActionTesterWithException();

            Action act = () => testee.Invoke(actionTester.TestAction);

            act.ShouldThrow<Exception>().Where(e => e.Message.Contains("originExceptionTest"));
        }

        [Test]
        public void GivenSimpleAction_WhenInvokedTwice_ShouldExecuteJustOnce()
        {
            var testee = new ApplicationDispatcher();
            
            var parallelActions =
                (from exec in Enumerable.Range(0, 100)
                    let at = new CountActionTester()
                    select new {Tester = at, Action = (Action) (() => testee.Invoke(at.TestAction))}).ToArray();

            Parallel.Invoke(parallelActions.Select(a => a.Action).ToArray());

            parallelActions.Any(a => a.Tester.ActionCalled != 1).Should().BeFalse();
        }

        public class ActionTester
        {
            public void TestAction()
            {
                WasCalled = true;
            }

            public bool WasCalled { get; private set; }
        }

        public class OrderedActionTester
        {
            public void TestAction1()
            {
                if (FirstCalledAction == 0)
                    FirstCalledAction = 1;
            }

            public void TestAction2()
            {
                if (FirstCalledAction == 0)
                    FirstCalledAction = 2;
            }

            public int FirstCalledAction { get; private set; }
        }

        public class ActionTesterWithException
        {
            public void TestAction()
            {
                throw new Exception("originExceptionTest");
            }
        }

        public class CountActionTester
        {
            private int _actionCalled;

            public void TestAction()
            {
                Interlocked.Increment(ref _actionCalled);
            }

            public int ActionCalled
            {
                get { return _actionCalled; }
                set { _actionCalled = value; }
            }
        }
    }
}
