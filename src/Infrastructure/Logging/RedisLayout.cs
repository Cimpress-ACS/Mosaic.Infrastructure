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


using log4net.Core;
using log4net.Layout;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

namespace VP.FF.PT.Common.Infrastructure.Logging
{
    /// <summary>
    /// The Redis Log4Net layout, provided by MSW Infrastructure squad
    /// </summary>
    public class RedisLayout : LayoutSkeleton
    {
        private readonly JsonSerializer _jsonSerializer = new JsonSerializer { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        public override void ActivateOptions() { }

        //Do not write out exception message after the json to the writer
        public override bool IgnoresException
        {
            get { return false; }
        }


        #region Public Properties used to inject information
        public string Instance { get; set; }

        public string Team { get; set; }

        public string LogOwner { get; set; }

        public string Organization { get; set; }
        #endregion

        public override void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            string loggerName = loggingEvent.LoggerName;
            Exception exception = loggingEvent.ExceptionObject;

            _jsonSerializer.Serialize(writer, new RedisJsonLog
            {
                Team = Team,
                LogOwner = LogOwner,
                Environment = Environment.MachineName,
                Host = Dns.GetHostEntry("").HostName,
                TimeStamp = DateTimeOffset.Now.ToString("o"),
                Type = loggingEvent.LoggerName,
                Message = loggingEvent.MessageObject,
                //Data = loggingEvent.MessageObject,
                ProcessId = Process.GetCurrentProcess().Id,
                MessageHash = loggingEvent.RenderedMessage.GetHashCode(),
                RenderedMessage = loggingEvent.RenderedMessage,
                ExceptionData = loggingEvent.ExceptionObject,
                Level = loggingEvent.Level.DisplayName,
                
                Application = loggingEvent.Domain,
                Executable = (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).FullName,
                Properties = loggingEvent.GetProperties(),

                //Custom fields
                Organization = Organization,
                ApplicationSubpart = loggerName,
                Exception = exception == null ? null : exception.ToString(),
                ExceptionMessage = exception == null ? null : exception.Message,
                ExceptionStackTrace = exception == null ? null : exception.StackTrace,
                ManagedThreadId = Thread.CurrentThread.ManagedThreadId,
                ManagedThreadName = Thread.CurrentThread.Name,
                Instance = Instance
            });
        }
    }
}
