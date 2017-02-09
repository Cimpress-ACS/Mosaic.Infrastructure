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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;
using Caliburn.Micro;
using Centigrade.Kit.StateMachine;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using Message = VP.FF.PT.Common.WpfInfrastructure.Screens.Model.Message;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    public class MessageViewModel : Screen
    {
        private readonly ObservableCollection<Message> _messages;
        private readonly StateMachine _stateMachine;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageViewModel"/> class.
        /// </summary>
        public MessageViewModel()
        {
            _stateMachine = new StateMachine();
            _stateMachine.StateChanged += _stateMachine_StateChanged;
            _messages = new ObservableCollection<Message>();
            MessagesView.SortDescriptions.Add(new SortDescription("MessageType", ListSortDirection.Descending));
            MessagesView.MoveCurrentToPosition(0);
            // set first tag of messagestate to lowest message priority
            StateMachine.GetState<MessageState>().Tag = MessageType.Consumable.ToString();
        }

        void _stateMachine_StateChanged(object sender, StateTransitioningEventArgs e)
        {
            Console.WriteLine("State Changed to "+e.ToState);
        }

        #region Properties

        public StateMachine StateMachine
        {
            get { return _stateMachine; }
        }

        public ICollectionView MessagesView
        {
            get { return CollectionViewSource.GetDefaultView(_messages); }
        }

        #endregion Properties

        #region States

        public IState NoMessageViewModelState
        {
            get { return StateMachine.GetState<NoMessageState>(); }
        }

        public IState MessageViewModelState
        {
            get { return StateMachine.GetState<MessageState>(); }
        }

        #region Nested type: MessageState

        private class MessageState : TaggedState
        {
        }

        #endregion

        #region Nested type: NoMessageState

        public class NoMessageState : State
        {
        }

        #endregion

        #endregion States

        #region StateTransitionCommands

        internal IStateTransitionCommand AddFirstMessageCommand
        {
            get
            {
                return StateMachine.GetStateTransitionCommand<NoMessageState, MessageState, Message>(
                    p =>
                        {
                            AddMessage(p);
                            // set tag to highest current message priority
                            StateMachine.GetState<MessageState>().Tag = GetHighestCurrentPriority().ToString();
                        });
            }
        }

        internal IStateTransitionCommand AddAnyMessageCommand
        {
            get
            {
                return StateMachine.GetStateTransitionCommand<MessageState, MessageState, Message>(
                    p =>
                        {
                            AddMessage(p);
                            // set tag to highest current message priority
                            StateMachine.GetState<MessageState>().Tag = GetHighestCurrentPriority().ToString();
                        });
            }
        }

        internal IStateTransitionCommand RemoveLastMessageCommand
        {
            get
            {
                return StateMachine.GetStateTransitionCommand<MessageState, NoMessageState, Message>(
                    p =>
                        {
                            RemoveMessage(p);
                            // set tag to highest current message priority
                            StateMachine.GetState<MessageState>().Tag = GetHighestCurrentPriority().ToString();
                        },
                    p => _messages.Count <= 1);
            }
        }

        internal IStateTransitionCommand RemoveAnyMessageCommand
        {
            get
            {
                return StateMachine.GetStateTransitionCommand<MessageState, MessageState, Message>(
                    p =>
                        {
                            RemoveMessage(p);
                            // set tag to highest current message priority
                            StateMachine.GetState<MessageState>().Tag = GetHighestCurrentPriority().ToString();
                        },
                    p => _messages.Count > 1);
            }
        }

        public ICommand RemoveMessageCommand
        {
            get { return StateMachine.ChooseStateTransitionCommand(RemoveLastMessageCommand, RemoveAnyMessageCommand); }
        }

        public ICommand AddMessageCommand
        {
            get { return StateMachine.ChooseStateTransitionCommand(AddFirstMessageCommand, AddAnyMessageCommand); }
        }

        #endregion StateTransitionCommands

        #region Methods

        private void AddMessage(Message message)
        {
            _messages.Add(message);
            Console.WriteLine("Message added. Messages: " + _messages.Count);
            MessagesView.MoveCurrentToPosition(0);
        }

        private void RemoveMessage(Message message)
        {
            foreach (Message item in _messages)
            {
                if (item.MessageId == message.MessageId &&
                    item.MessageType == message.MessageType)
                {
                    _messages.Remove(item);
                    Console.WriteLine("Message removed. Messages: "+_messages.Count);
                    return;
                }
            }
        }

        /// <summary>
        /// Gets the highest current priority.
        /// </summary>
        /// <returns>
        /// The highest current message type.
        /// </returns>
        private MessageType GetHighestCurrentPriority()
        {
            var highestPriority = MessageType.Consumable;
            foreach (Message message in _messages)
            {
                if (message.MessageType > highestPriority)
                {
                    highestPriority = message.MessageType;
                }
            }

            return highestPriority;
        }

        #endregion Methods
    }
}
