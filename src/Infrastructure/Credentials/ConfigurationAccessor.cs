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


using System.ComponentModel.Composition;
using System.Configuration;
using System.Runtime.InteropServices;

namespace VP.FF.PT.Common.Infrastructure.Credentials
{
    /// <summary>
    /// The <see cref="ConfigurationAccessor"/> is capable of accessing the <see cref="ConfigurationManager"/>
    /// and provide specific information from it.
    /// </summary>
    [Export(contractType: typeof(IProvideConfiguration))]
    public class ConfigurationAccessor: IProvideConfiguration
    {
        /// <summary>
        /// gets the configuratoin of the section described in the section parameter
        /// </summary>
        /// <typeparam name="T">Configuration Class to pass back</typeparam>
        /// <param name="section">section definition of the configuration to read</param>
        /// <returns></returns>
        public T GetConfiguration<T>(string section) where T: class 
        {
            return ConfigurationManager.GetSection(section) as T;
        }
    }
}
