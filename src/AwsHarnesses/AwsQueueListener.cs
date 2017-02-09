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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VP.FF.PT.Common.Infrastructure;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace Cimpress.ACS.AwsHarnesses
{
    /// <summary>
    /// This listener impl. needs to have the MessageVisibilityTimeout configured to 0 seconds (default is 30 seconds). Because otherwise StopListening (cancelling/disposing) won't work properly.
    /// Because of this the limitation of this implementation is: Only one QueueListener per SQS-Queue is allowed!
    /// </summary>
    /// <typeparam name="T">The message body will be deserialized into this type.</typeparam>
    [Export(typeof(IMessageQueueListener<>))]
    [Export(typeof(IMessageQueueManagement))]
    [Export(typeof(IShutdown))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AwsQueueListener<T> : IMessageQueueListener<T>, IMessageQueueManagement, IShutdown
    {
        private readonly ILogger _logger;
        private IAmazonSQS _sqs;
        private ReceiveMessageRequest _receiveMessageRequest;
        private string _queueUrl;
        private CancellationTokenSource _cancellationTokenSource;

        public event EventHandler<T> DataReceived;

        private readonly AwsUsedQueuesRepository _usedQueuesRepository;
        private bool _isConnectedToSns;

        [ImportingConstructor]
        public AwsQueueListener(ILogger logger, AwsUsedQueuesRepository usedQueuesRepository)
        {
            _logger = logger;
            _usedQueuesRepository = usedQueuesRepository;
            _logger.Init(GetType());
        }

        /// <summary>
        /// Initializes the specified aws queue URL. This needs to be called before starting the listener.
        /// </summary>
        /// <param name="awsQueueUrl">The aws queue URL.</param>
        /// <param name="awsRegionEndpoint">The region endpoint.</param>
        /// <param name="awsAccessKey">The aws access key.</param>
        /// <param name="awsSecretKey">The aws secret key.</param>
        /// <param name="isConnectedToSns">if set to <c>true</c> it assumes a SNS message structure and parsed only the "message" part of it. This is an optional feature, default is false.</param>
        /// <param name="longPollTimeout">The long poll timeout in seconds, AWS default is 20 seconds.</param>
        public void Initialize(string awsQueueUrl, string awsRegionEndpoint, string awsAccessKey, string awsSecretKey,
            bool isConnectedToSns = false, int longPollTimeout = 20)
        {
            VerifySingleListenerPerQueue(awsQueueUrl);

            var region = RegionEndpointHelper.Parse(awsRegionEndpoint);

            _queueUrl = awsQueueUrl;
            _isConnectedToSns = isConnectedToSns;
            _receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = awsQueueUrl,
                WaitTimeSeconds = longPollTimeout,
                MaxNumberOfMessages = 1 // make sure 1 is the default value
            };

            AWSCredentials credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
            _sqs = new AmazonSQSClient(credentials, region);

            var response = _sqs.GetQueueAttributes(awsQueueUrl, new List<string> {"QueueArn"});
            if (response.HttpStatusCode != HttpStatusCode.OK)
                throw new AwsQueueListenerException(string.Format(
                    "Cannot initialize AwsQueueListener. HttpStatus code {0} for queue {1} in region {2}", response.HttpStatusCode, awsQueueUrl, region));

            _logger.InfoFormat("AwsSyncQueueListener initialized for queue {0} in {1}", awsQueueUrl, region);
        }

        /// <exception cref="AwsQueueListenerException">AwsQueueListener not initialized</exception>
        public Task StartListening()
        {
            if (_sqs == null)
                throw new AwsQueueListenerException("AwsQueueListener not initialized");

            _cancellationTokenSource = new CancellationTokenSource();

            return Task.Factory.StartNew<Task>(Listen, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <exception cref="AwsQueueListenerException">AwsQueueListener not initialized</exception>
        public void StopListening()
        {
            if (_sqs == null)
                throw new AwsQueueListenerException("AwsQueueListener not initialized");

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
            }

            _logger.Info("stopped listening to SQS");
        }

        private async Task Listen()
        {
            _logger.Info("started listening to SQS");

            while (true)
            {
                if (_cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    ReceiveMessageResponse receiveMessageResponse =
                        await _sqs.ReceiveMessageAsync(_receiveMessageRequest, _cancellationTokenSource.Token);

                    foreach (Message message in receiveMessageResponse.Messages)
                    {
                        try
                        {
                            JToken body = JsonConvert.DeserializeObject<JToken>(message.Body);

                            T dataReceived;

                            if (_isConnectedToSns)
                            {
                                var bodyMessage = (string) body["Message"];
                                dataReceived = JsonConvert.DeserializeObject<T>(bodyMessage);
                            }
                            else
                            {
                                dataReceived = JsonConvert.DeserializeObject<T>(message.Body);
                            }

                            OnPackDataReceived(dataReceived);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(string.Format(
                                "Unable to parse or process the message: {0}, error: {1}. See InnerException.",
                                message.Body, e.Message), e);
                        }
                        finally
                        {
                            DeleteMessage(message);
                        }
                    }
                }
                catch (AmazonSQSException e)
                {
                    _logger.ErrorFormat("AWS exception caught while listening. Message: {0}", e.Message);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception e)
                {
                    _logger.Error("Unexpected exception while listening.", e);
                }
            }
        }

        private void DeleteMessage(Message message)
        {
            if (message != null)
            {
                string messageReceiptHandle = message.ReceiptHandle;

                _logger.Debug("Deleting the message(s).");
                DeleteMessageRequest deleteRequest = new DeleteMessageRequest
                {
                    QueueUrl = _queueUrl,
                    ReceiptHandle = messageReceiptHandle
                };
                _sqs.DeleteMessageAsync(deleteRequest);
            }
            else
            {
                _logger.Debug("No message has been received...");
            }
        }

        public bool IsListening
        {
            get { return _sqs != null && !_cancellationTokenSource.IsCancellationRequested; }
        }

        protected virtual void OnPackDataReceived(T data)
        {
            if (DataReceived != null)
                DataReceived(this, data);
        }

        public void Shutdown()
        {
            StopListening();

            if (_sqs != null)
                _sqs.Dispose();
        }

        /// <exception cref="AwsQueueListenerException">AwsQueueListener not initialized</exception>
        public void Purge()
        {
            if (_sqs == null)
                throw new AwsQueueListenerException("AwsQueueListener not initialized");

            try
            {
                _sqs.PurgeQueueAsync(_queueUrl);
            }
            catch (PurgeQueueInProgressException e)
            {
                // ignore because only one PurgeQueue operation is allowed every 60 seconds
                _logger.Debug(e.Message, e);
            }
            catch (QueueDoesNotExistException e)
            {
                _logger.Error(e.Message, e);
            }
        }

        private void VerifySingleListenerPerQueue(string addNewAwsQueueUrl)
        {
            if (_usedQueuesRepository.UsedQueues.Contains(addNewAwsQueueUrl))
                throw new AwsQueueListenerException("Only one AwsQueueListener instance per queue allowed. Queue: " + addNewAwsQueueUrl);

            _usedQueuesRepository.UsedQueues.Add(addNewAwsQueueUrl);
        }
    }

    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AwsUsedQueuesRepository
    {
        public readonly ConcurrentBag<string> UsedQueues = new ConcurrentBag<string>();
    }
}
