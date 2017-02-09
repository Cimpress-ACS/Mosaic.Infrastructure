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
    /// Facade which imports ILoggerExtensions and uses them for logging.
    /// This implementation should be the onliest export for ILogger.
    /// To implement a new logger in an specific application the ILoggerExtension has to be used.
    /// </summary>
    [Export(typeof(ILogger))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AggregatedLogger : ILogger
    {
        private readonly IList<ILogger> _loggers = new List<ILogger>();

        public AggregatedLogger()
        {
        }

        [ImportingConstructor]
        public AggregatedLogger([ImportMany] IEnumerable<ILoggerExtension> loggerExtensions)
        {
            loggerExtensions.ForEach(l => _loggers.Add(l));
        }

        public string LoggerName
        {
            get
            {
                if (_loggers.Any())
                    return _loggers.First().LoggerName;

                return string.Empty;
            }
        }

        /// <summary>
        /// Adds a logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public void AddLogger(ILogger logger)
        {
            _loggers.Add(logger);
        }

        public void Init(Type caller)
        {
            foreach (var logger in _loggers)
                logger.Init(caller);
        }

        public void Init(string name)
        {
            foreach (var logger in _loggers)
                logger.Init(name);
        }

        public void Info(string message)
        {
            foreach (var logger in _loggers)
                logger.Info(message);
        }

        public void Info(string message, Exception exception)
        {
            foreach (var logger in _loggers)
                logger.Info(message, exception);
        }

        public void InfoFormat(string message, params object[] parameters)
        {
            foreach (var logger in _loggers)
                logger.InfoFormat(message, parameters);
        }

        public void Debug(string message)
        {
            foreach (var logger in _loggers)
                logger.Debug(message);
        }

        public void Debug(string message, Exception exception)
        {
            foreach (var logger in _loggers)
                logger.Debug(message, exception);
        }

        public void DebugFormat(string message, params object[] parameters)
        {
            foreach (var logger in _loggers)
                logger.DebugFormat(message, parameters);
        }

        public void Warn(string message)
        {
            foreach (var logger in _loggers)
                logger.Warn(message);
        }

        public void Warn(string message, Exception exception)
        {
            foreach (var logger in _loggers)
                logger.Warn(message, exception);
        }

        public void WarnFormat(string message, params object[] parameters)
        {
            foreach (var logger in _loggers)
                logger.WarnFormat(message, parameters);
        }

        public void Error(string message)
        {
            foreach (var logger in _loggers)
                logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            foreach (var logger in _loggers)
                logger.Error(message, exception);
        }

        public void ErrorFormat(string message, params object[] parameters)
        {
            foreach (var logger in _loggers)
                logger.ErrorFormat(message, parameters);
        }
    }
}
