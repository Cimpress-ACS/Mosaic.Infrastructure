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

namespace VP.FF.PT.Common.Infrastructure.Logging
{
    public interface ILogger
    {
        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        /// <value>
        /// The name of the logger.
        /// </value>
        String LoggerName { get; }

        /// <summary>
        /// Creates the underlying logger if not exist and sets the name of calling context by type. 
        /// </summary>
        /// <param name="caller">Type of calling assembly. Normally you have to pass just "this" here.</param>
        void Init(Type caller);

        /// <summary>
        /// Creates the underlying logger if not exist and sets the name.
        /// </summary>
        /// <param name="callerName">Pass the name of the calling assembly here. Use GetType() method for this.</param>
        void Init(string callerName);

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Info(String message);
        
        /// <summary>
        /// Logs an info message
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Info(String message, Exception exception);

        /// <summary>
        /// Logs an info message with a format string.
        /// </summary>
        /// <param name="message">The message. Can include substitution characters {0} {1} etc.</param>
        /// <param name="parameters">The parameters for string substitution.</param>
        void InfoFormat(String message, params object[] parameters);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Debug(String message);

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Debug(String message, Exception exception);

        /// <summary>
        /// Logs a debug message with a format string.
        /// </summary>
        /// <param name="message">The message. Can include substitution characters {0} {1} etc.</param>
        /// <param name="parameters">The parameters for string substitution.</param>
        void DebugFormat(String message, params object[] parameters);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Warn(string message);

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Warn(String message, Exception exception);

        /// <summary>
        /// Logs a warning message with a format string.
        /// </summary>
        /// <param name="message">The message. Can include substitution characters {0} {1} etc.</param>
        /// <param name="parameters">The parameters for string substitution.</param>
        void WarnFormat(String message, params object[] parameters);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        void Error(String message);

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        void Error(String message, Exception exception);

        /// <summary>
        /// Logs an error message with a format string.
        /// </summary>
        /// <param name="message">The message. Can include substitution characters {0} {1} etc.</param>
        /// <param name="parameters">The parameters for string substitution.</param>
        void ErrorFormat(string message, params object[] parameters);
    }
}
