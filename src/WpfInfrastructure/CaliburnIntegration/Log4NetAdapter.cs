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
using Caliburn.Micro;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration
{
    public class Log4NetAdapter : ILog
    {
        private readonly ILogger _logger;

        public Log4NetAdapter(ILogger logger)
        {
            _logger = logger;
        }

        public ILog Init(Type type)
        {
            _logger.Init(type);
            return this;
        }

        public void Info(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            _logger.Warn(string.Format(format, args));
        }

        public void Error(Exception exception)
        {
            _logger.Error(string.Empty, exception);
        }
    }
}
