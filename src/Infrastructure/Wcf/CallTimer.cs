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


using System.Diagnostics;
using System.ServiceModel.Dispatcher;
using VP.FF.PT.Common.Infrastructure.Logging.Structured;

namespace VP.FF.PT.Common.Infrastructure.Wcf
{
    /// <summary>
    /// The actual call timer to log WCF call time performance.
    /// </summary>
    internal class CallTimer : IParameterInspector
    {
        private readonly string _contractName;
        private readonly bool _isClient;
        private readonly IPerformanceLogger<WcfPerformanceData> _log;

        public CallTimer(string contractName, bool isClient)
        {
            _contractName = contractName;
            _isClient = isClient;
            _log = new PerformanceLogger<WcfPerformanceData>("WcfExecutionTime");
        }

        /// <summary>
        /// Stops the timer and logs the elapsed WcfExecutionTime
        /// </summary>
        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            var measurement = ((Stopwatch) correlationState).ElapsedMilliseconds;
            var data = new WcfPerformanceData(_contractName, operationName, _isClient ? "Client" : "Server", measurement);
            _log.Log(data);
        }

        /// <summary>
        /// Starts a timer before the call begins
        /// </summary>
        public object BeforeCall(string operationName, object[] inputs)
        {
            return Stopwatch.StartNew();
        }
    }
}
