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
using log4net;

namespace VP.FF.PT.Common.Infrastructure.Logging
{
    /// <summary>
    /// Default file logger implementation using Log4Net.
    /// Do not allocate this class directly, use ILogger and MEF instead.
    /// </summary>
    [Export(typeof(ILoggerExtension))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Log4NetLogger : ILoggerExtension
    {
        static Log4NetLogger()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        private ILog _internalLogger;

        public void Init(Type caller)
        {
            _internalLogger = CreateLogger(caller.FullName);
        }

        public void Init(string name)
        {
            _internalLogger = CreateLogger(name);
        }

        public string LoggerName
        {
            get { return InternalLogger.Logger.Name; }
        }

        public void Info(string message)
        {
            InternalLogger.Info(message);
        }

        public void Info(string message, Exception exception)
        {
            InternalLogger.Info(message, exception);

            if (exception.InnerException != null)
            {
                Info(message, exception.InnerException);
            }
        }

        public void InfoFormat(string message, params object[] parameters)
        {
            InternalLogger.InfoFormat(message, parameters);
        }

        public void Debug(string message)
        {
            InternalLogger.Debug(message);
        }

        public void Debug(string message, Exception exception)
        {
            InternalLogger.Debug(message, exception);

            if (exception.InnerException != null)
            {
                Debug(message, exception.InnerException);
            }
        }

        public void DebugFormat(string message, params object[] parameters)
        {
            InternalLogger.DebugFormat(message, parameters);
        }

        public void Warn(string message)
        {
            InternalLogger.Warn(message);
        }

        public void Warn(string message, Exception exception)
        {
            InternalLogger.Warn(message, exception);

            if (exception.InnerException != null)
            {
                Warn(message, exception.InnerException);
            }
        }

        public void WarnFormat(string message, params object[] parameters)
        {
            InternalLogger.WarnFormat(message, parameters);
        }

        public void Error(string message)
        {
            InternalLogger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            InternalLogger.Error(message, exception);

            if (exception.InnerException != null)
            {
                Error(message, exception.InnerException);
            }
        }

        public void ErrorFormat(string message, params object[] parameters)
        {
            InternalLogger.ErrorFormat(message, parameters);
        }

        private ILog InternalLogger
        {
            get
            { 
                if (_internalLogger == null)
                {
                    var method = new CallingMethod(typeof(Log4NetLogger));
                    _internalLogger = CreateLogger(method.TypeNameFull);
                }

                return _internalLogger;
            }
        }

        private ILog CreateLogger(string name)
        {
            var logger = LogManager.GetLogger(name);
            return logger;
        }
    }
}
