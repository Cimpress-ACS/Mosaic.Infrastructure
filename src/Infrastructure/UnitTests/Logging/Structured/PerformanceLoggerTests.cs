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
using System.Linq;
using System.Threading;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.Logging.Structured;

namespace VP.FF.PT.Common.Infrastructure.UnitTests.Logging.Structured
{
    [TestFixture]
    public class PerformanceLoggerTests
    {
        [Test]
        [Ignore("Only used for testing the system.")]
        public void Test()
        {
            var logger = new PerformanceLogger<PerformanceData>("unittests");

            var rnd = new Random();

            var dataValue = new [] {"Action 1", "Action 2", "Action 3"};

            foreach (var i in Enumerable.Range(0, 10))
            {
                var performanceData = new PerformanceData(dataValue[i % 3], (float) rnd.NextDouble());
                logger.Log(performanceData);
                Thread.Sleep(rnd.Next(0, 10));
            }
            Thread.Sleep(100);
        }
    }
}
