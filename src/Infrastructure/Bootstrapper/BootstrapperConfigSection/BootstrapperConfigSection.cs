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


using System.Configuration;

namespace VP.FF.PT.Common.Infrastructure.Bootstrapper.BootstrapperConfigSection
{
      //<bootstrapper rootFolderName="LineControl" mefAssemblyNamePattern="VP.FF.PT*.dll">
      //  <ignorePaths>
      //    <add name="\UnitTests\" />
      //    <add name="\obj\" />
      //    <add name="\LineControlInstaller\" />
      //    <add name="\AutoTest.Net\" />
      //    <add name=".vshost.exe" />
      //    <add name=".mm.dll" />
      //  </ignorePaths>
      //  <additionalLookupPaths>
      //    <add name="../../../../../MyFolder"/>
      //  </additionalLookupPaths>
      //</bootstrapper>
    public class BootstrapperConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("rootFolderName", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string RootFolderName
        {
            get { return (string)base["rootFolderName"]; }
            set { base["rootFolderName"] = value; }
        }

        [ConfigurationProperty("mefAssemblyNamePattern", DefaultValue = "", IsRequired = true)]
        public string MefAssemblyNamePattern
        {
            get { return (string)base["mefAssemblyNamePattern"]; }
            set { base["mefAssemblyNamePattern"] = value; }
        }

        [ConfigurationProperty("ignorePaths")]
        public IgnorePathsConfigCollection IgnorePaths
        {
            get { return (IgnorePathsConfigCollection)base["ignorePaths"]; }
        }

        [ConfigurationProperty("additionalLookupPaths", IsRequired = false)]
        public AdditionalLookupPathsConfigCollection AdditionalLookupPaths
        {
            get { return (AdditionalLookupPathsConfigCollection)base["additionalLookupPaths"]; }
        }
    }
}
