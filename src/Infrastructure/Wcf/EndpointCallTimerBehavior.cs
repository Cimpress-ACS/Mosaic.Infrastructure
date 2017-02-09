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


using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace VP.FF.PT.Common.Infrastructure.Wcf
{
    /// <summary>
    /// WCF behavior that logs performance data of WCF calls.
    /// </summary>
    public class EndpointCallTimerBehavior : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters) {}

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            IParameterInspector timer = new CallTimer(endpoint.Contract.Name, true);
            foreach (ClientOperation op in clientRuntime.Operations)
            {
                op.ParameterInspectors.Add(timer);
            }
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
