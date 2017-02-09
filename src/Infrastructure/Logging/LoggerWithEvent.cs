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
using System.Globalization;
using System.Threading;
using log4net.Core;
using log4net.Util;

namespace VP.FF.PT.Common.Infrastructure.Logging
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class LoggerWithEvent : ILoggerExtension
    {
        public event EventHandler<LogEventArgs> LogEvent;

        public void Init(Type caller)
        {
            LoggerName = caller.Name;
        }

        public void Init(string name)
        {
            LoggerName = name;
        }

        public string LoggerName { get; private set; }

        public void Info(string message)
        {
            RaiseLogEvent(Level.Info, message);
        }

        public void Info(string message, Exception exception)
        {
            RaiseLogEvent(Level.Info, message, exception);
        }

        public void InfoFormat(string message, params object[] parameters)
        {
            RaiseLogEvent(Level.Info, new SystemStringFormat(CultureInfo.InvariantCulture, message, parameters).ToString());
        }

        public void Debug(string message)
        {
            RaiseLogEvent(Level.Debug, message);
        }

        public void Debug(string message, Exception exception)
        {
            RaiseLogEvent(Level.Debug, message, exception);
        }

        public void DebugFormat(string message, params object[] parameters)
        {
            RaiseLogEvent(Level.Debug, new SystemStringFormat(CultureInfo.InvariantCulture, message, parameters).ToString());
        }

        public void Warn(string message)
        {
            RaiseLogEvent(Level.Warn, message);
        }

        public void Warn(string message, Exception exception)
        {
            RaiseLogEvent(Level.Warn, message, exception);
        }

        public void WarnFormat(string message, params object[] parameters)
        {
            RaiseLogEvent(Level.Warn, new SystemStringFormat(CultureInfo.InvariantCulture, message, parameters).ToString());
        }

        public void Error(string message)
        {
            RaiseLogEvent(Level.Error, message);
        }

        public void Error(string message, Exception exception)
        {
            RaiseLogEvent(Level.Error, message, exception);
        }

        public void ErrorFormat(string message, params object[] parameters)
        {
            RaiseLogEvent(Level.Error, new SystemStringFormat(CultureInfo.InvariantCulture, message, parameters).ToString());
        }

        private void RaiseLogEvent(Level level, string message, Exception exception = null)
        {
            if (LogEvent != null)
            {
                LogEvent(this, new LogEventArgs(new LoggingEvent(
                    new LoggingEventData
                    {
                        ThreadName = Thread.CurrentThread.Name,
                        Domain = AppDomain.CurrentDomain.FriendlyName,
                        TimeStamp = DateTime.Now,
                        Level = level,
                        Message = message,
                        LoggerName = LoggerName,
                        ExceptionString = exception != null ? exception.ToString() : string.Empty
                    })));
            }
        }
    }
}
