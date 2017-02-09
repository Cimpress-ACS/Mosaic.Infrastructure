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


using log4net;

namespace VP.FF.PT.Common.Infrastructure.Logging.Structured
{
    public class PerformanceLogger<T> : IPerformanceLogger<T>
    {
        private readonly ILog _log;

        public PerformanceLogger(string logType)
        {
            _log = LogManager.GetLogger("StructuredLogger." + logType);
        }

        public virtual void Log(T data)
        {
            _log.Info(data);
        }
    }
}
