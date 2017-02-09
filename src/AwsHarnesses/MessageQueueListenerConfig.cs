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

namespace Cimpress.ACS.AwsHarnesses
{
    public class MessageQueueListenerConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageQueueListenerConfig"/> class
        /// with a default long poll timeout of 20 seconds.
        /// </summary>
        public MessageQueueListenerConfig()
        {
            PollTimeout = TimeSpan.FromSeconds(20);
        }

        public string QueueUrl { get; set; }
        public string RegionEndpoint { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public TimeSpan PollTimeout { get; set; }
    }
}
