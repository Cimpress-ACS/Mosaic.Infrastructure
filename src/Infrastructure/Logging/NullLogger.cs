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
    /// <summary>
    /// A logger that does not log at all, but allows the option to use this logger instead of a null object.
    /// </summary>
    public class NullLogger : ILogger
    {
        public string LoggerName { get { return "NullLogger"; } }

        public void Init(Type caller) {}

        public void Init(string callerName) {}

        public void Info(string message) {}

        public void Info(string message, Exception exception) {}

        public void InfoFormat(string message, params object[] parameters) {}

        public void Debug(string message) {}

        public void Debug(string message, Exception exception) {}

        public void DebugFormat(string message, params object[] parameters) {}

        public void Warn(string message) {}

        public void Warn(string message, Exception exception) {}

        public void WarnFormat(string message, params object[] parameters) {}

        public void Error(string message) {}

        public void Error(string message, Exception exception) {}

        public void ErrorFormat(string message, params object[] parameters) {}
    }
}
