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

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Model
{
    /// <summary>
    /// Captures changes of elements in KeyValueUnitViewModel. The key uniquely identifies the KeyValueUnit.
    /// The old and new value are captured (can e.g. be gathered in a changelist).
    /// </summary>
    public class KeyValueChange
    {
        public string Key { get; private set; }
        public string Name { get; private set; }
        public object NewValue { get; private set; }
        public object OldValue { get; private set; }
        public Type Type { get; private set; }

        public KeyValueChange(string key, string name, object newValue, object oldValue, Type type)
        {
            Key = key;
            Name = name;
            NewValue = newValue;
            OldValue = oldValue;
            Type = type;
        }
    }
}
