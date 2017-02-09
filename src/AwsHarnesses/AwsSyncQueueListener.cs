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
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    /// Alternative QueueListener implementation of AWS SQS which is not exposed to the MEF container.
    /// It does NOT use AWS ..Async methods because there are reproducible issues with that.
    /// To still provide a fast shutdown (dispose) this implementation uses a magic-dummy-termination-message to cancel a synchronous long poll (20s).
    /// This implementation supports having multiple listener-instances per SQS-Queue.
    /// </summary>
    /// <typeparam name="T">The message body will be deserialized into this type.</typeparam>
    public class AwsSyncQueueListener<T> : IShutdown, IMessageQueueListener<T>, IMessageQueueManagement
    {
        private readonly ILogger _logger;
        private IAmazonSQS _sqs;
        private ReceiveMessageRequest _receiveMessageRequest;
        private string _queueUrl;
        private Task _task;
        private bool _isConnectedToSns;
        private const string MagicCancellationMessageBody = "8F42F6BD-0893-415E-AE43-B90E17B1D7B4-magiccancelmessage";

        public event EventHandler<T> DataReceived;

        [ImportingConstructor]
        public AwsSyncQueueListener(ILogger logger)
        {
            _logger = logger;

            IsListening = false;
        }

        /// <summary>
        /// Initializes the specified aws queue URL. This needs to be called before starting the listener.
        /// </summary>
        /// <param name="awsQueueUrl">The aws queue URL.</param>
        /// <param name="awsRegionEndpoint">The region endpoint.</param>
        /// <param name="awsAccessKey">The aws access key.</param>
        /// <param name="awsSecretKey">The aws secret key.</param>
        /// <param name="isConnectedToSns">if set to <c>true</c> it assumes a SNS message structure and parsed only the "message" part of it. This is an optional feature, default is false.</param>
        /// <param name="longPollTimeout">The long poll timeout in seconds.</param>
        public void Initialize(string awsQueueUrl, string awsRegionEndpoint, string awsAccessKey, string awsSecretKey, 
                                bool isConnectedToSns = false, int longPollTimeout = 20)
        {
            var region = RegionEndpointHelper.Parse(awsRegionEndpoint);

            AWSCredentials credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);
            _sqs = new AmazonSQSClient(credentials, region);
            _queueUrl = awsQueueUrl;
            _isConnectedToSns = isConnectedToSns;
            _receiveMessageRequest = new ReceiveMessageRequest { QueueUrl = awsQueueUrl, WaitTimeSeconds = longPollTimeout };

            _logger.InfoFormat("AwsSyncQueueListener initialized for queue {0} in {1}", awsQueueUrl, region);
        }

        /// <exception cref="AwsQueueListenerException">AwsSyncQueueListener not initialized</exception>
        public Task StartListening()
        {
            if (_sqs == null)
                throw new AwsQueueListenerException("AwsSyncQueueListener not initialized");

            if (!IsListening)
            {
                IsListening = true;

                _task = Task.Factory.StartNew(Listen, TaskCreationOptions.LongRunning);
                return _task;
            }

            _logger.Info("start will not be started because is currently running");
            return Task.FromResult(-1);
        }

        private void Listen()
        {
            _logger.Info("started listening to SQS");

            while (IsListening)
            {
                try
                {
                    ReceiveMessageResponse receiveMessageResponse = _sqs.ReceiveMessage(_receiveMessageRequest);

                    foreach (Message message in receiveMessageResponse.Messages)
                    {
                        if (message.Body.Equals(MagicCancellationMessageBody))
                        {
                            _logger.Debug("received cancellation message");
                            DeleteMessages(receiveMessageResponse.Messages);
                            IsListening = false;
                            return;
                        }

                        try
                        {
                            JToken body = JsonConvert.DeserializeObject<JToken>(message.Body);

                            T dataReceived;

                            if (_isConnectedToSns)
                            {
                                var bodyMessage = (string)body["Message"];
                                dataReceived = JsonConvert.DeserializeObject<T>(bodyMessage);
                            }
                            else
                            {
                                dataReceived = JsonConvert.DeserializeObject<T>(message.Body);
                            }
                            
                            OnPackDataReceived(dataReceived);
                        }
                        catch (Exception ex)
                        {
                            _logger.ErrorFormat("Unable to parse the message: {0}, error: {1}",
                                message.Body, ex.Message);
                        }
                        finally
                        {
                            DeleteMessages(receiveMessageResponse.Messages);
                        }
                    }
                }
                catch (AmazonSQSException ex)
                {
                    _logger.Error("AWS exception caught while listening" + ex.Message);
                }
            }
        }

        private void DeleteMessages(List<Message> messages)
        {
            foreach(var message in messages)
            {
                string messageReceiptHandle = message.ReceiptHandle;

                DeleteMessageRequest deleteRequest = new DeleteMessageRequest
                {
                    QueueUrl = _queueUrl,
                    ReceiptHandle = messageReceiptHandle
                };
                _sqs.DeleteMessageAsync(deleteRequest);
            }
        }

        /// <exception cref="AwsQueueListenerException">AwsSyncQueueListener not initialized</exception>
        /// <exception cref="InvalidMessageContentsException">
        ///             The message contains characters outside the allowed set.
        ///             </exception>
        /// <exception cref="UnsupportedOperationException">
        ///             Error code 400. Unsupported operation.
        ///             </exception>
        public void StopListening()
        {
            if (_sqs == null)
                throw new AwsQueueListenerException("AwsSyncQueueListener not initialized");

            if (!IsListening)
                return;

            IsListening = false;

            // send a fake message to cancel the long-poll blocking call
            _sqs.SendMessage(new SendMessageRequest(_queueUrl, MagicCancellationMessageBody));

            _task.Wait();
            _logger.Info("stopped listening to QNS");
        }

        public bool IsListening { get; private set; }

        protected virtual void OnPackDataReceived(T data)
        {
            var handler = DataReceived;
            if (handler != null)
                handler(this, data);
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
                throw new AwsQueueListenerException("AwsSyncQueueListener not initialized");

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
    }
}
