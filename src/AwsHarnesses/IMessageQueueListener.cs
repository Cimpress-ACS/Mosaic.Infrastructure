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
using System.Threading.Tasks;

namespace Cimpress.ACS.AwsHarnesses
{
    public interface IMessageQueueListener<T>
    {
        event EventHandler<T> DataReceived;
        bool IsListening { get; }

        /// <summary>
        /// Initializes the specified aws queue URL. This needs to be called before starting the listener.
        /// </summary>
        /// <param name="awsQueueUrl">The aws queue URL.</param>
        /// <param name="awsRegionEndpoint">The region endpoint.</param>
        /// <param name="awsAccessKey">The aws access key.</param>
        /// <param name="awsSecretKey">The aws secret key.</param>
        /// <param name="queueIsSubscribedToSns">if set to <c>true</c> it assumes a SNS message structure and parsed only the "message" part of it. This is an optional feature, default is false.</param>
        /// <param name="longPollTimeout">The long poll timeout in seconds, AWS default is 20 seconds.</param>
        void Initialize(string awsQueueUrl, string awsRegionEndpoint, string awsAccessKey, string awsSecretKey, bool queueIsSubscribedToSns = false, int longPollTimeout = 20);
        Task StartListening();
        void StopListening();
    }
}
