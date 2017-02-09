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


namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Model
{
    /// <summary>
    /// Container for a forcable PLC variable.
    /// </summary>
    public class ForcingTag : Tag
    {
        /// <summary>
        /// The given real value from the PLC.
        /// In case of an "input" tag the value might represent a sensor signal.
        /// In case of an "output" tag the value represents the PLC calculated value.
        /// The real value is not changeable.
        /// </summary>
        public object RealValue { get; set; }

        /// <summary>
        /// The value from user to overwrite the real value if forcing is enabled.
        /// </summary>
        public object ForceValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether forcing is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if ForceValue should overwrite the RealValue; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled { get; set; }
    }
}
