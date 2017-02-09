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
using System.Threading;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace VP.FF.PT.Common.Infrastructure
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CommandLineHelper
    {
        private readonly ILogger _logger;

        [ImportingConstructor]
        public CommandLineHelper(ILogger logger)
        {
            _logger = logger;
            _logger.Init(GetType());
        }

        public void ExecuteCommandSync(object command)
        {
            try
            {
                // create the ProcessStartInfo using "cmd" as the program to be run,
                // and "/c " as the parameters.
                // Incidentally, /c tells cmd that we want it to execute the command that follows,
                // and then exit.
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + command)
                    {
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                // The following commands are needed to redirect the standard output.

                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();

                string result = proc.StandardOutput.ReadToEnd();
                _logger.Info(result);
            }
            catch (Exception e)
            {
                // Log the exception
                _logger.Error("Cannot execute command " + command, e);
            }
        }

        public void ExecuteCommandAsync(string command)
        {
            try
            {
                //Asynchronously start the Thread to process the Execute command request.
                var objThread = new Thread(ExecuteCommandSync)
                {
                    IsBackground = true
                };

                objThread.Start(command);
            }
            catch (ThreadStartException e)
            {
                _logger.Error("Cannot execute command " + command, e);
            }
            catch (ThreadAbortException e)
            {
                _logger.Error("Cannot execute command " + command, e);
            }
            catch (Exception e)
            {
                _logger.Error("Cannot execute command " + command, e);
            }
        }
    }
}
