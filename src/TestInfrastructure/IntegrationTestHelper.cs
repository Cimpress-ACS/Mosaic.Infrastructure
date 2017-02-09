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
using System.ComponentModel.Composition.Hosting;
using System.Threading.Tasks;
using FluentAssertions;
using VP.FF.PT.Common.Infrastructure.Bootstrapper;

namespace VP.FF.PT.Common.TestInfrastructure
{
    /// <summary>
    /// Provides some tools for integration unit tests.
    /// </summary>
    public class IntegrationTestHelper
    {
        private MefBootstrapper _bootstrapper;

        private LoggingHelper _loggingHelper;

        public IntegrationTestHelper()
        {
        }

        public IntegrationTestHelper(CompositionContainer container)
        {
            Container = container;
        }

        public CompositionContainer Container { get; private set; }
        
        public LoggingHelper LoggingHelper
        {
            get
            {
                if (_loggingHelper == null)
                    _loggingHelper = new LoggingHelper();

                return _loggingHelper;
            }

            set
            {
                _loggingHelper = value;
            }
        }

        /// <summary>
        /// Runs the specified test with an optional timeout and
        /// in case of a test-fail it will add useful informations to the message (ERROR in logfile).
        /// It also sets the timeout to maximum if a Debugger is attached.
        /// </summary>
        /// <param name="testToRun">Delegate for the test to run.</param>
        /// <param name="timeout">The timeout.</param>
        public void Run(Action testToRun, TimeSpan timeout = default(TimeSpan))
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("detected debug mode. will set test timeout to maximum...");
                timeout = TimeSpan.MaxValue;
            }

            try
            {
                var task = new Task(testToRun, TaskCreationOptions.LongRunning);
                task.Start();

                if (timeout == default(TimeSpan) || System.Diagnostics.Debugger.IsAttached)
                {
                    task.Wait();
                }
                else
                {
                    var result = task.Wait(timeout);

                    if (!result)
                    {
                        string logfileTail = LoggingHelper.GetLastLogMessage();

                        throw new IntegrationTestFailedException(string.Format(
                            "Test failed because it exceeded the maximum timeout of {0}. Logfile tail: {1}", timeout, logfileTail));
                    }
                }

            }
            catch (Exception e)
            {
                if (LoggingHelper.HasErrors())
                {

                    var ex = new IntegrationTestFailedException(string.Format("Detected ERROR in Logfile: {0}", LoggingHelper.GetFirstErrorMessage()), e);
                    throw ex;
                }

                throw;
            }
        }

        public void StartupMosaic(MefBootstrapper bootstrapper)
        {
            _bootstrapper = bootstrapper;
            _bootstrapper.AdditionalCatalogs.Catalogs.Add(new AssemblyCatalog(bootstrapper.GetType().Assembly));

            _bootstrapper.Run();

            Container = _bootstrapper.Container;

            LoggingHelper = new LoggingHelper();
        }

        public void ShutdownMosaic()
        {
            _bootstrapper.Stop();
        }

        public void VerifyNoLogErrors()
        {
            LoggingHelper.HasErrors()
                .Should()
                .BeFalse("detected error log message during Helios startup: " + LoggingHelper.GetFirstErrorMessage());
        }

        public void VerifyNoLogWarnings()
        {
            LoggingHelper.HasWarnings()
                .Should()
                .BeFalse("detected warning log message during Helios startup: " + LoggingHelper.GetFirstWarningMessage());
        }
    }
}
