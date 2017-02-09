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
using VP.FF.PT.Common.Infrastructure.Logging;
using bbv.Common.StateMachine;
using bbv.Common.StateMachine.Extensions;
using bbv.Common.StateMachine.Internals;

namespace VP.FF.PT.Common.Infrastructure
{
    /// <summary>
    /// A logger extension for bbv state machine to log Debug informations like state transition and to log Error in case of exceptions.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    public class StateMachineLoggerExtension<TState, TEvent> : ExtensionBase<TState, TEvent> where TState : IComparable where TEvent : IComparable
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineLoggerExtension{TState, TEvent}"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public StateMachineLoggerExtension(ILogger logger)
        {
            _logger = logger;
        }

        public override void SwitchedState(IStateMachineInformation<TState, TEvent> stateMachine, IState<TState, TEvent> oldState, IState<TState, TEvent> newState)
        {
            base.SwitchedState(stateMachine, oldState, newState);

            if (oldState != null)
                _logger.Debug("StateMachine " + stateMachine.Name + " switched from " + oldState.ToString() + " to " + newState.ToString());
            else
                _logger.Debug("StateMachine " + stateMachine.Name + " switched to " + newState.ToString());
        }

        public override void FiringEvent(IStateMachineInformation<TState, TEvent> stateMachine, ref TEvent eventId, ref object[] eventArguments)
        {
            base.FiringEvent(stateMachine, ref eventId, ref eventArguments);
            _logger.Debug("StateMachine " + stateMachine.Name + " fires event " + eventId);
        }

        public override void InitializedStateMachine(IStateMachineInformation<TState, TEvent> stateMachine, TState initialState)
        {
            base.InitializedStateMachine(stateMachine, initialState);
            _logger.Debug("Initialized StateMachine " + stateMachine.Name + " with initial state " + initialState);
        }

        public override void StartedStateMachine(IStateMachineInformation<TState, TEvent> stateMachine)
        {
            base.StartedStateMachine(stateMachine);
            _logger.Debug("Started StateMachine " + stateMachine.Name);
        }

        public override void StoppedStateMachine(IStateMachineInformation<TState, TEvent> stateMachine)
        {
            base.StoppedStateMachine(stateMachine);
            _logger.Debug("Stopped StateMachine " + stateMachine.Name);
        }

        public override void EventQueued(IStateMachineInformation<TState, TEvent> stateMachine, TEvent eventId, object[] eventArguments)
        {
            base.EventQueued(stateMachine, eventId, eventArguments);
            //_logger.Debug("Queued event " + eventId + " in StateMachine " + stateMachine.Name );
        }

        public override void EventQueuedWithPriority(IStateMachineInformation<TState, TEvent> stateMachine, TEvent eventId, object[] eventArguments)
        {
            base.EventQueuedWithPriority(stateMachine, eventId, eventArguments);
            _logger.Debug("Queued event " + eventId + " with priority in StateMachine " + stateMachine.Name);
        }

        public override void HandledEntryActionException(IStateMachineInformation<TState, TEvent> stateMachine, IState<TState, TEvent> state, IStateContext<TState, TEvent> stateContext, Exception exception)
        {
            base.HandledEntryActionException(stateMachine, state, stateContext, exception);
            _logger.Error("EntryActionException handled in StateMachine " + stateMachine.Name + " in state " + state.ToString());
            _logger.Error(exception.Message);
        }

        public override void HandledExitActionException(IStateMachineInformation<TState, TEvent> stateMachine, IState<TState, TEvent> state, IStateContext<TState, TEvent> stateContext, Exception exception)
        {
            base.HandledExitActionException(stateMachine, state, stateContext, exception);
            _logger.Error("ExitActionException handled in StateMachine " + stateMachine.Name + " in state " + state.ToString());
            _logger.Error(exception.Message);
        }

        public override void HandledTransitionException(IStateMachineInformation<TState, TEvent> stateMachine, ITransition<TState, TEvent> transition, ITransitionContext<TState, TEvent> transitionContext, Exception exception)
        {
            base.HandledTransitionException(stateMachine, transition, transitionContext, exception);
            _logger.Error("TransitionException handled in StateMachine " + stateMachine.Name + " in transition " + transition.Source.ToString() + "->" + transition.Target.ToString());
            _logger.Error(exception.Message);
        }

        public override void HandlingGuardException(IStateMachineInformation<TState, TEvent> stateMachine, ITransition<TState, TEvent> transition, ITransitionContext<TState, TEvent> transitionContext, ref Exception exception)
        {
            base.HandlingGuardException(stateMachine, transition, transitionContext, ref exception);
            _logger.Error("GuardException handled in StateMachine " + stateMachine.Name + " in transition " + transition.Source.ToString() + "->" + transition.Target.ToString());
            _logger.Error(exception.Message);
        }
    }
}
