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


using log4net.Appender;
using log4net.Core;
using StackExchange.Redis;
using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using log4net.Util;

namespace VP.FF.PT.Common.Infrastructure.Logging
{
    /// <summary>
    /// The Redis Log4Net appender, provided by MSW Infrastructure squad
    /// </summary>
    class RedisAppender : AppenderSkeleton
    {
        //const int RemotePort = 6379;
        internal string RemoteAddress { get; set; }
		internal int RemotePort { get; set; }
		
        string RemoteUrl
        {
            get { return string.Format("{0}:{1}", RemoteAddress, RemotePort); }
        }

        readonly object _clientLockObject = new object();
        ConnectionMultiplexer _client;

        protected override bool RequiresLayout
        {
            get { return true; }
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();
			
            if (RemotePort < IPEndPoint.MinPort || RemotePort > IPEndPoint.MaxPort) 
            {
                    throw SystemInfo.CreateArgumentOutOfRangeException("value", RemotePort, 
                        string.Format("The value specified is less than {0} or greater than {1}.",
                            IPEndPoint.MinPort.ToString(NumberFormatInfo.InvariantInfo), IPEndPoint.MaxPort.ToString(NumberFormatInfo.InvariantInfo)));
            }

            if (RemoteAddress == null)
            {
                throw new ArgumentException("Remote address of the location must be specified.");
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            Task.Run(() =>
            {
                lock (_clientLockObject)
                {
                    if (_client == null || !_client.IsConnected)
                    {
                        CloseConnectionMultiplexer(_client, true);
                        _client = ConnectionMultiplexer.Connect(RemoteUrl);
                    }
                    _client.GetDatabase().ListRightPush("logstash", new RedisValue[] { RenderLoggingEvent(loggingEvent) });
                }
            }).ContinueWith(t =>
                ErrorHandler.Error(string.Format("Unable to send logging event to Redis host ({0}).", RemoteUrl),
                    t.Exception, ErrorCode.WriteFailure),
                TaskContinuationOptions.OnlyOnFaulted);
        }

        protected override void OnClose()
        {
            lock (_clientLockObject)
            {
                CloseConnectionMultiplexer(_client, false);
            }
            base.OnClose();
        }

        private void CloseConnectionMultiplexer(IConnectionMultiplexer connectionMultiplexer,
            bool allowCommandsToComplete)
        {
            if (connectionMultiplexer == null)
            {
                return;
            }

            connectionMultiplexer.CloseAsync(allowCommandsToComplete).ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    ErrorHandler.Error(
                        string.Format("Unable to successfully close the connection to the Redis host ({0}).", RemoteUrl),
                        t.Exception, ErrorCode.WriteFailure);
                }
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
