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


using System.Collections.Generic;
using System.Threading.Tasks;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Services
{
    public class ControllerCommanderNullObject : ICommandControllers
    {
        public Task ActivateSimulation(string moduleName, int controllerId, bool propagate)
        {
            return Task.Run(() => { });
        }

        public Task DeactivateSimulation(string moduleName, int controllerId, bool propagate)
        {
            return Task.Run(() => { });
        }

        public Task SetControllerToMode(string moduleName, int controllerId, Controller.Mode mode)
        {
            return Task.Run(() => { });
        }

        public Task ExecuteCommand(string moduleName, int controllerId, string commandName)
        {
            return Task.Run(() => { });
        }

        public Task SaveChanges(string moduleName, int controllerId, IEnumerable<KeyValueChange> changes)
        {
            return Task.Run(() => { });
        }
    }
}
