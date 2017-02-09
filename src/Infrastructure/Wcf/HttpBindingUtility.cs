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


using System.ServiceModel;

namespace VP.FF.PT.Common.Infrastructure.Wcf
{
    /// <summary>
    /// Note that a second channel is needed for WSDualHttpBinding when having eventing (which means a second port is needed and a "clientBaseAddress").
    /// </summary>
    public class HttpBindingUtility
    {
        static HttpBindingUtility()
        {
#if(DEBUG)
    // TODO: set different timeouts for debugging
#else

#endif
        }

        public static BasicHttpBinding CreateBasicHttpBinding()
        {
            var httpBinding = new BasicHttpBinding();

            httpBinding.HostNameComparisonMode = HostNameComparisonMode.Exact;

            return httpBinding;
        }

        public static EndpointAddress CreateEndpointAddress(string serviceAddress)
        {
            return new EndpointAddress(string.Format("http://{0}", serviceAddress));
        }

        public static WSDualHttpBinding CreateWsDualHttpBinding()
        {
            var dualBinding = new WSDualHttpBinding();

            dualBinding.HostNameComparisonMode = HostNameComparisonMode.Exact;
            dualBinding.BypassProxyOnLocal = false;
            dualBinding.TransactionFlow = false;


            return dualBinding;
        }
    }
}
