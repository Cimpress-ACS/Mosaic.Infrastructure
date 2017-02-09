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
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web.Script.Serialization;
using log4net.Core;
using log4net.Layout;

namespace VP.FF.PT.Common.Infrastructure.Logging
{
    public class GenericJSLogLayout : LayoutSkeleton
    {
        private readonly JavaScriptSerializer _javaScriptSerializer = new JavaScriptSerializer();
        private readonly Assembly _entryAssembly;

        public string Instance { get; set; }

        public GenericJSLogLayout()
        {
            // cache data that are re-used for every message
            _javaScriptSerializer = new JavaScriptSerializer();
            _entryAssembly = Assembly.GetEntryAssembly();
        }

        public override void ActivateOptions() { }

        public override void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            string loggerName = loggingEvent.LoggerName;
            // weak conventions by now to figure out whether we want to log the rendered message, or raw data
            bool isStructured = loggerName.StartsWith("StructuredLogger.");
            Exception exception = loggingEvent.ExceptionObject;

            var logData = new
            {
                Application = loggingEvent.Domain,
                Level = loggingEvent.Level.DisplayName,
                Message = !isStructured ? loggingEvent.RenderedMessage : null,
                Data = isStructured ? loggingEvent.MessageObject : null,

                //Custom fields that apply for Mosaic only
                ApplicationSubpart = loggerName,
                Exception = exception == null ? null : exception.ToString(),
                ExceptionMessage = exception == null ? null : exception.Message,
                ExceptionStackTrace = exception == null ? null : exception.StackTrace,
                Executable = (_entryAssembly ?? Assembly.GetExecutingAssembly()).FullName,
                ManagedThreadId = Thread.CurrentThread.ManagedThreadId,
                ManagedThreadName = Thread.CurrentThread.Name,
                MosaicInstance = Instance,
                Properties = loggingEvent.GetProperties()
            };
            var serializedLogData = _javaScriptSerializer.Serialize(logData);
            writer.WriteLine(serializedLogData);
        }

        public override bool IgnoresException { get { return false; } }
    }
}
