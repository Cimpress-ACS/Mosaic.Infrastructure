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
    public class ConsoleOutLogger : ILogger
    {
        public string LoggerName { get; private set; }

        public void Init(Type caller)
        {
            LoggerName = caller.Name;
        }

        public void Init(string callerName)
        {
            LoggerName = callerName;
        }

        public void Info(string message)
        {
            Print(message);
        }

        public void Info(string message, Exception exception)
        {
            Print(message, exception);
        }

        public void InfoFormat(string message, params object[] parameters)
        {
            Print(message, parameters);
        }

        public void Debug(string message)
        {
            Print(message);
        }

        public void Debug(string message, Exception exception)
        {
            Print(message, exception);
        }

        public void DebugFormat(string message, params object[] parameters)
        {
            Print(message, parameters);
        }

        public void Warn(string message)
        {
            Print(message);
        }

        public void Warn(string message, Exception exception)
        {
            Print(message, exception);
        }

        public void WarnFormat(string message, params object[] parameters)
        {
            Print(message, parameters);
        }

        public void Error(string message)
        {
            Print(message);
        }

        public void Error(string message, Exception exception)
        {
            Print(message, exception);
        }

        public void ErrorFormat(string message, params object[] parameters)
        {
            Print(message, parameters);
        }

        private void Print(string message)
        {
            Console.Out.WriteLine("[{0}]{1}", LoggerName, message);
        }

        private void Print(string message, params object[] parameters)
        {
            Print(string.Format(message, parameters));
        }

        private void Print(string message, Exception exception)
        {
            Print(string.Format("{0}{1}{2}", message, Environment.NewLine, exception));
        }
    }
}
