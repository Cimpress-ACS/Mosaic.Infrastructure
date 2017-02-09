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
using System.ComponentModel.Composition;
using System.Linq;

namespace VP.FF.PT.Common.Infrastructure.Logging
{
    /// <summary>
    /// The <see cref="InMemoryLogsContainer"/> can store log messages comming from any logger
    /// and expose them for any use. Useful for unit testing.
    /// </summary>
    //[Export(typeof(InMemoryLogsContainer))]
    //[Export(typeof(IProvideLogMessages))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class InMemoryLogsContainer : IProvideLogMessages
    {
        private const int DefaultMessageLimit = 1500;

        private readonly object _lock = new object();

        private readonly int _messageLimit;
        private readonly Queue<LogMessage> _innerMessages;
        private IReadOnlyCollection<LogMessage> _snapshot;

        public InMemoryLogsContainer()
            : this(DefaultMessageLimit)
        {
        }

        /// <summary>
        /// Initialize a new <see cref="InMemoryLogsContainer"/> instance.
        /// </summary>
        /// <param name="messageLimit">The message limit.</param>
        public InMemoryLogsContainer(int messageLimit = DefaultMessageLimit)
        {
            _messageLimit = messageLimit;
            _innerMessages = new Queue<LogMessage>(messageLimit);
            _snapshot = _innerMessages.ToReadOnly();
        }

        /// <summary>
        /// Gets all messages which were logged from one of the specified <paramref name="emitters"/>.
        /// </summary>
        /// <param name="emitters">The emitters to get the messages from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="LogMessage"/> instances.</returns>
        public IEnumerable<LogMessage> GetMessages(params string[] emitters)
        {
            IEnumerable<string> emitterEnumerable = emitters;
            return GetMessages(emitterEnumerable);
        }

        /// <summary>
        /// Gets all messages which were logged from one of the specified <paramref name="emitters"/>.
        /// </summary>
        /// <param name="emitters">The emitters to get the messages from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="LogMessage"/> instances.</returns>
        public IEnumerable<LogMessage> GetMessages(IEnumerable<string> emitters)
        {
            IReadOnlyCollection<string> readOnlyEmiters = emitters.ToReadOnly();
            if (readOnlyEmiters.IsNullOrEmpty())
                return new LogMessage[] { };
            return _snapshot.Where(m => readOnlyEmiters.Any(e => string.Equals(e, m.Emitter)));
        }

        /// <summary>
        /// Adds the specified <paramref name="message"/> to the inner log.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void AddMessage(LogMessage message)
        {
            if (message == null)
                return;
            lock (_lock)
            {
                if (_innerMessages.Count == _messageLimit)
                    _innerMessages.Dequeue();
                _innerMessages.Enqueue(message);
                _snapshot = _innerMessages.ToReadOnly();
            }
        }
    }
}
