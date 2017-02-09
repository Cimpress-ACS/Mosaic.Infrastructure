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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

namespace VP.FF.PT.Common.Infrastructure.Concurrency
{
    /// <summary>
    /// Global dispatcher to invoke actions on a single thread. Its a polling based action executer helper.
    /// Concurrency safe alternative to timers, use this on a single thread and call
    /// it regularly.
    /// Using the ApplicationDispatcher everything can be synchronized to eliminate all concurrency issues.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ApplicationDispatcher : IApplicationDispatcher
    {
        private readonly object _lock = new object();

        private class ActionMetaData
        {
            public Action Action;
            public DateTime AddedTime;
            public int Delay;
            public int PollsToWait;
            public int RemainingPolls;
        }

        private class ActionMetaDataWithParameter : ActionMetaData
        {
            public Action<object> ActionWithArgument;
            public object Argument;
        }

        private readonly IList<ActionMetaData> _actions = new List<ActionMetaData>();

        /// <summary>
        /// Call this method regularly to check for new actions to execute.
        /// </summary>
        public void ExecuteInvokes()
        {
            var now = DateTime.Now;

            var invokedActions = new Collection<ActionMetaData>();


            lock (_lock)
            {
                IList<ActionMetaData> actionsToInvoke = new List<ActionMetaData>(_actions);

                foreach (var action in actionsToInvoke)
                {
                    if (action.PollsToWait == 0 && (now - action.AddedTime).TotalMilliseconds >= action.Delay)
                    {
                        try
                        {
                            var actionWithParameter = action as ActionMetaDataWithParameter;
                            if (actionWithParameter != null)
                            {
                                actionWithParameter.ActionWithArgument(actionWithParameter.Argument);
                            }
                            else
                            {
                                action.Action();
                            }

                            invokedActions.Add(action);
                        }
                        catch (Exception e)
                        {
                            var msg = string.Format("A delayed action throwed an exception. Delay was {0}ms, InnerMessage was: '{1}'", action.Delay, e.Message);
                            throw new Exception(msg, e);
                        }
                    }
                    else if (action.PollsToWait > 0 && --action.RemainingPolls == 0)
                    {
                        try
                        {
                            action.Action.Invoke();
                            invokedActions.Add(action);
                        }
                        catch (Exception e)
                        {
                            var msg = string.Format("A delayed action throwed an exception. Polls to wait count was {0}, InnerMessage was: '{1}'",
                                action.PollsToWait, e.Message);
                            throw new Exception(msg, e);
                        }
                    }
                }

                foreach (var invokedAction in invokedActions)
                {
                    _actions.Remove(invokedAction);
                }
            }
        }

        public void Invoke(Action action)
        {
            lock (_lock)
            {
                _actions.Add(new ActionMetaData
                             {
                                 Action = action,
                                 AddedTime = DateTime.Now,
                                 PollsToWait = 0,
                                 RemainingPolls = 0
                             });
            }

            ExecuteInvokes();
        }

        public void Invoke(Action<object> action, object argument)
        {
            lock (_lock)
            {
                _actions.Add(new ActionMetaDataWithParameter
                {
                    ActionWithArgument = action,
                    Argument = argument,
                    AddedTime = DateTime.Now,
                    PollsToWait = 0,
                    RemainingPolls = 0
                });
            }

            ExecuteInvokes();
        }

        /// <summary>
        /// Adds the action with specified delay time in ms.
        /// </summary>
        /// <param name="delay">The delay time in milliseconds.</param>
        /// <param name="action">The action to execute.</param>
        public void AddDelayedAction(int delay, Action action)
        {
            lock (_lock)
            {
                _actions.Add(new ActionMetaData
                             {
                                 Action = action,
                                 AddedTime = DateTime.Now,
                                 Delay = delay
                             });
            }
        }

        public void AddActionAfterPolls(int numberOfPolls, Action action)
        {
            if (numberOfPolls <= 0)
            {
                throw new ArgumentException("0 or negative numbers not allowed", "numberOfPolls");
            }

            lock (_lock)
            {
                _actions.Add(new ActionMetaData
                             {
                                 Action = action,
                                 AddedTime = DateTime.Now,
                                 PollsToWait = numberOfPolls,
                                 RemainingPolls = numberOfPolls
                             });
            }
        }

        /// <summary>
        /// Cancells all planned actions.
        /// </summary>
        public void CancellAll()
        {
            lock (_lock)
            {
                _actions.Clear();
            }
        }
    }
}
