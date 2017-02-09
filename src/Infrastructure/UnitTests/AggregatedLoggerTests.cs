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
using System.ComponentModel.Composition.Hosting;
using FluentAssertions;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    // we export this only for testing purposes, to have access to the test counter
    [Export]
    [Export(typeof(ILoggerExtension))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CustomTestLogger : ILoggerExtension
    {
        public int InfoLogCount { get; set; }
        public int DebugLogCount { get; set; }
        public int WarnLogCount { get; set; }
        public int ErrorLogCount { get; set; }

        public string LoggerName { get; private set; }
        public void Init()
        {
        }

        public void Init(Type caller)
        {
        }

        public void Init(string name)
        {
        }

        public void Info(string message)
        {
            InfoLogCount++;
        }

        public void Info(string message, Exception exception)
        {
            InfoLogCount++;
        }

        public void InfoFormat(string message, params object[] parameters)
        {
            InfoLogCount++;
        }

        public void Debug(string message)
        {
            DebugLogCount++;
        }

        public void Debug(string message, Exception exception)
        {
            DebugLogCount++;
        }

        public void DebugFormat(string message, params object[] parameters)
        {
            DebugLogCount++;
        }

        public void Warn(string message)
        {
            WarnLogCount++;
        }

        public void Warn(string message, Exception exception)
        {
            WarnLogCount++;
        }

        public void WarnFormat(string message, params object[] parameters)
        {
            WarnLogCount++;
        }

        public void Error(string message)
        {
            ErrorLogCount++;
        }

        public void Error(string message, Exception exception)
        {
            ErrorLogCount++;
        }

        public void ErrorFormat(string message, params object[] parameters)
        {
            ErrorLogCount++;
        }
    }

    // test if 
    [TestFixture]
    public class AggregatedLoggerTests
    {
        ILogger _testeeAggregatedLogger;
        CustomTestLogger _customLogger;

        [SetUp]
        public void Setup()
        {
            ComposeAndGetTestExports(out _testeeAggregatedLogger, out _customLogger);
        }

        [Test]
        public void WhenImportILogger_AggregatedLoggerIsInjected()
        {
            _testeeAggregatedLogger.Should().BeOfType<AggregatedLogger>();
        }

        [Test]
        public void WhenLogInfo_CustomTestLoggerLoggedItCorrectly()
        {
            _testeeAggregatedLogger.Info("info log");

            _customLogger.InfoLogCount.Should().Be(1, "info line has been logged before");
            _customLogger.DebugLogCount.Should().Be(0, "no debug line has been logged");
            _customLogger.WarnLogCount.Should().Be(0, "no warn line has been logged");
            _customLogger.ErrorLogCount.Should().Be(0, "no error line has been logged");
        }

        [Test]
        public void WhenLogDebug_CustomTestLoggerLoggedItCorrectly()
        {
            _testeeAggregatedLogger.Debug("debug log");

            _customLogger.InfoLogCount.Should().Be(0, "no line has been logged");
            _customLogger.DebugLogCount.Should().Be(1, "debug line has been logged before");
            _customLogger.WarnLogCount.Should().Be(0, "no warn line has been logged");
            _customLogger.ErrorLogCount.Should().Be(0, "no error line has been logged");
        }

        [Test]
        public void WhenLogWarnings_CustomTestLoggerLoggedItCorrectly()
        {
            _testeeAggregatedLogger.Warn("warn log");
            _testeeAggregatedLogger.Warn("another warn log");

            _customLogger.InfoLogCount.Should().Be(0, "no info line has been logged");
            _customLogger.DebugLogCount.Should().Be(0, "no debug line has been logged");
            _customLogger.WarnLogCount.Should().Be(2, "two warning lines has been logged before");
            _customLogger.ErrorLogCount.Should().Be(0, "no error line has been logged");
        }

        [Test]
        public void WhenLogError_CustomTestLoggerLoggedItCorrectly()
        {
            _testeeAggregatedLogger.Error("error log");

            _customLogger.InfoLogCount.Should().Be(0, "no info line has been logged");
            _customLogger.DebugLogCount.Should().Be(0, "no debug line has been logged");
            _customLogger.WarnLogCount.Should().Be(0, "no warn line has been logged");
            _customLogger.ErrorLogCount.Should().Be(1, "error line has been logged before");
        }

        private void ComposeAndGetTestExports<T, F>(out T first, out F second)
        {
            var types = new TypeCatalog(
                typeof(CustomTestLogger),
                typeof(AggregatedLogger));

            var container = new CompositionContainer(new AggregateCatalog(types));

            var batch = new CompositionBatch();
            batch.AddPart(container);

            container.Compose(batch);

            first = container.GetExportedValue<T>();
            second = container.GetExportedValue<F>();
        }
    }
}
