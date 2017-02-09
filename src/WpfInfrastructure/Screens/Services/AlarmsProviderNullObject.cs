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
using System.Collections.Generic;
using System.Threading.Tasks;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Services
{
    public class AlarmsProviderNullObject : IProvideAlarms
    {
        public Task RequestAlarms(ICollection<string> module)
        {
            return Task.Run(() => {});
        }

        public Task SubscribeForAlarmChanges(ICollection<string> module, Action<IEnumerable<Alarm>, IEnumerable<Alarm>> handler)
        {
            return Task.Run(() => {});
        }

        public Task UnsubscribeFromAlarmChanges(ICollection<string> module, Action<IEnumerable<Alarm>, IEnumerable<Alarm>> handler)
        {
            return Task.Run(() => { });
        }
    }
}
