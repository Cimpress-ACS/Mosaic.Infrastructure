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
using System.ComponentModel.Composition;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace Cimpress.ACS.AwsHarnesses
{
    [Export(typeof(IMessagePublisher))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AwsTopicPublisher : IMessagePublisher
    {
        private readonly ILogger _logger;
        private AmazonSimpleNotificationServiceClient _client;
        private string _awsTopicArn;

        [ImportingConstructor]
        public AwsTopicPublisher(ILogger logger)
        {
            _logger = logger;
            _logger.Init(GetType());
        }

        /// <summary>
        /// Initializes the specified aws queue URL.
        /// </summary>
        /// <param name="awsTopicArn">The aws topic ARN.</param>
        /// <param name="regionEndpoint">The region endpoint.</param>
        /// <param name="awsAccessKey">The aws access key.</param>
        /// <param name="awsSecretKey">The aws secret key.</param>
        public void Initialize(string awsTopicArn, string regionEndpoint, string awsAccessKey, string awsSecretKey)
        {
            _awsTopicArn = awsTopicArn;

            try
            {
                AWSCredentials credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
                _client = new AmazonSimpleNotificationServiceClient(credentials, RegionEndpointHelper.Parse(regionEndpoint));
            }
            catch (Exception e)
            {
                _logger.Error("Couldn't initialize AwsTopicPublisher!", e);
            }
        }

        public void Publish<T>(T message, string subject)
        {
            try
            {
                if (string.IsNullOrEmpty(subject))
                {
                    _client.Publish(new PublishRequest
                    {
                        Message = JsonConvert.SerializeObject(message),
                        TopicArn = _awsTopicArn
                    });
                }
                else
                {
                    _client.Publish(new PublishRequest
                    {
                        Subject = subject,
                        Message = JsonConvert.SerializeObject(message),
                        TopicArn = _awsTopicArn
                    });
                }
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Couldn't publish {0} to topic {1}", subject, _awsTopicArn, e);
            }
        }
    }
}
