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
using System.ComponentModel.Composition;

namespace VP.FF.PT.Common.Infrastructure.Logging
{
    /// <summary>
    /// The <see cref="InMemoryLogger"/> implements the <see cref="ILoggerExtension"/> interface
    /// and writes all logs to an in memory log message container. Useful for unit testing.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class InMemoryLogger : ILogger
    {
        private readonly InMemoryLogsContainer _logsContainer;
        private string _name;

        /// <summary>
        /// Initializes a new <see cref="InMemoryLogger"/> instance.
        /// </summary>
        /// <param name="logsContainer">The in memory logs container to add the log messages to.</param>
        [ImportingConstructor]
        public InMemoryLogger(InMemoryLogsContainer logsContainer)
        {
            _logsContainer = logsContainer;
            _name = string.Empty;
        }

        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        public string LoggerName
        {
            get { return _name; }
        }

        /// <summary>
        /// Creates the underlying logger if not exist and sets the name of calling context by type. 
        /// </summary>
        /// <param name="caller">Type of calling assembly. Normally you have to pass just "this" here.</param>
        public void Init(Type caller)
        {
            _name = caller.FullName;
        }

        /// <summary>
        /// Creates the underlying logger if not exist and sets the name.
        /// </summary>
        /// <param name="callerName">Pass the name of the calling assembly here. Use GetType() method for this.</param>
        public void Init(string callerName)
        {
            _name = callerName;
        }

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            Info(message, null);
        }

        /// <summary>
        /// Logs an info message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(string message, Exception exception)
        {
            AddMessageToLogContainer(LogMessage.CategoryEnum.Info, message, exception);
        }

        /// <summary>
        /// Logs an info message with a format string.
        /// </summary>
        /// <param name="message">The message. Can include substitution characters {0} {1} etc.</param>
        /// <param name="parameters">The parameters for string substitution.</param>
        public void InfoFormat(string message, params object[] parameters)
        {
            string formatedMessage = string.Format(message, parameters);
            Info(formatedMessage, null);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            Debug(message, null);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(string message, Exception exception)
        {
            AddMessageToLogContainer(LogMessage.CategoryEnum.Debug, message, exception);
        }

        public void DebugFormat(string message, params object[] parameters)
        {
            Debug(string.Format(message, parameters));
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            Warn(message, null);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(string message, Exception exception)
        {
            AddMessageToLogContainer(LogMessage.CategoryEnum.Warning, message, exception);
        }

        public void WarnFormat(string message, params object[] parameters)
        {
            Warn(string.Format(message, parameters));
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            Error(message, null);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(string message, Exception exception)
        {
            AddMessageToLogContainer(LogMessage.CategoryEnum.Error, message, exception);
        }

        /// <summary>
        /// Logs an error message with a format string.
        /// </summary>
        /// <param name="message">The message. Can include substitution characters {0} {1} etc.</param>
        /// <param name="parameters">The parameters for string substitution.</param>
        public void ErrorFormat(string message, params object[] parameters)
        {
            string formatedMessage = string.Format(message, parameters);
            Error(formatedMessage, null);
        }

        private void AddMessageToLogContainer(LogMessage.CategoryEnum category, string text, Exception exception = null)
        {
            _logsContainer.AddMessage(Message(category, text, exception));
        }

        private LogMessage Message(LogMessage.CategoryEnum category, string text, Exception exception = null)
        {
            return new LogMessage(_name, category, text, exception);
        }
    }
}
