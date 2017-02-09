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
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace VP.FF.PT.Common.WpfInfrastructure.Threading
{
    public class MessageNotifier : IDisposable
    {
        private class MessageBoxInfos
        {
            public string Message { get; set; }
            public string Caption { get; set; }
            public MessageBoxImage MessageBoxImage { get; set; }
        }

        private Task _task;
        private CancellationTokenSource _cts;
        private Dispatcher _dispatcher;
        private ConcurrentQueue<MessageBoxInfos> _messages = new ConcurrentQueue<MessageBoxInfos>();
        private bool _disposed;

        public MessageNotifier(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void Push(string message, string caption, MessageBoxImage messageBoxImage)
        {
            _messages.Enqueue(new MessageBoxInfos() 
            { 
                Caption = caption,
                Message = message,
                MessageBoxImage = messageBoxImage
            });
        }

        public void Run()
        {
            Stop();

            _cts = new CancellationTokenSource();
            _task = new Task(NotifyMessages, _cts.Token, _cts.Token);
            _task.Start();
        }
                
        public void Stop()
        {
            if (_cts != null && _task != null)
            {
                try
                {
                    _cts.Cancel();

                    // use timeout to prevent deadlock if messagebox was not confirmed by user
                    _task.Wait(3000); 
                }
                finally
                {
                    _cts = null;
                    _task = null;
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Stop();
                _disposed = true;
            }
            else
                throw new ObjectDisposedException(this.GetType().Name);
        }

        private void NotifyMessages(object obj)
        {
            var token = (CancellationToken)obj;
            MessageBoxInfos messageBoxInfos;

            while (!token.IsCancellationRequested)
            {
                if (_messages.TryDequeue(out messageBoxInfos))
                {
                    // don't use InvokeAsync
                    // --> the variable "messageBoxInfos" could be set to NULL
                    //     because in the async case the TryDequeue method will be
                    //     called again after the _dispatcher.InvokeAsync() call 
                    _dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(
                            messageBoxInfos.Message,
                            messageBoxInfos.Caption,
                            MessageBoxButton.OK, 
                            messageBoxInfos.MessageBoxImage);
                    });    
                }

                Thread.Sleep(500);
            }
        }
    }
}
