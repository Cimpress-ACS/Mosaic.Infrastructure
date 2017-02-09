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
using log4net;
using log4net.Appender;
using log4net.Core;

namespace VP.FF.PT.Common.TestInfrastructure
{
    /// <summary>
    /// Adds a memory appender in order to listen to log entries.
    /// Useful to verify if there were warnings or errors.
    /// </summary>
    public class LoggingHelper
    {
        private readonly MemoryAppender _memoryAppender;

        public LoggingHelper()
        {
            _memoryAppender = LogManager.GetRepository().GetAppenders().First(a => a.Name == "MemoryAppender") as MemoryAppender;
        }

        public bool HasErrors()
        {
            return _memoryAppender.GetEvents().Any(e => e.Level == Level.Error);
        }

        public string GetFirstErrorMessage()
        {
            var errorEvent = _memoryAppender.GetEvents().FirstOrDefault(e => e.Level == Level.Error);

            if (errorEvent != null)
            {
                return errorEvent.RenderedMessage + Environment.NewLine + errorEvent.GetExceptionString();
            }

            return string.Empty;
        }

        public bool HasWarnings()
        {
            return _memoryAppender.GetEvents().Any(e => e.Level == Level.Warn);
        }

        public string GetFirstWarningMessage()
        {
            var errorEvent = _memoryAppender.GetEvents().FirstOrDefault(e => e.Level == Level.Warn);

            if (errorEvent != null)
            {
                return errorEvent.RenderedMessage;
            }

            return string.Empty;
        }

        public bool ContainsLogMessage(string message)
        {
            return _memoryAppender.GetEvents().Any(e => e.RenderedMessage.ToUpperInvariant().Contains(message.ToUpperInvariant()));
        }

        public bool ContainsLogMessage(string message, Level ofLevel)
        {
            return _memoryAppender.GetEvents().Any(e => e.RenderedMessage.ToUpperInvariant().Contains(message.ToUpperInvariant()) && e.Level == ofLevel);
        }

        public string GetLastLogMessage()
        {
            var lastOrDefault = _memoryAppender.GetEvents().LastOrDefault();
            if (lastOrDefault != null)
                return lastOrDefault.RenderedMessage;

            return string.Empty;
        }
    }
}
