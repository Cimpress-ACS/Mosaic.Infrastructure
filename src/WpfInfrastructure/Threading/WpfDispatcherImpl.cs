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
using System.Threading.Tasks;
using System.Windows;

namespace VP.FF.PT.Common.WpfInfrastructure.Threading
{
    [Export(typeof(IDispatcher))]
    public class WpfDispatcherImpl : IDispatcher
    {
        public void Dispatch(Action actionToDispatch)
        {
            Application.Current.Dispatcher.Invoke(actionToDispatch);
        }

        public async Task Dispatch(Func<Task> actionToDispatch)
        {
            await await Application.Current.Dispatcher.InvokeAsync(actionToDispatch);
        }

        public async Task<TResult> Dispatch<TResult>(Func<Task<TResult>> functionToDispatch)
        {
            return await await Application.Current.Dispatcher.InvokeAsync(functionToDispatch);
        }
    }
}
