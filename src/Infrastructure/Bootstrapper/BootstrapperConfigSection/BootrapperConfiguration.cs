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
    public class BootrapperConfiguration
    {
        public BootrapperConfiguration()
        {
            // read configuration
            var configSection = ConfigurationManager.GetSection("bootstrapper") as BootstrapperConfigSection;

            RootFolderName = configSection.RootFolderName;
            MEFAssemblyNamePattern = configSection.MefAssemblyNamePattern;

            IgnorePathPatterns = new string[configSection.IgnorePaths.Count];

            int i = 0;
            foreach (NameValueConfigurationElement element in configSection.IgnorePaths)
            {
                IgnorePathPatterns[i++] = element.Name;
            }

            AdditionalLookupPaths = new string[configSection.AdditionalLookupPaths.Count];

            int j = 0;
            foreach (NameValueConfigurationElement element in configSection.AdditionalLookupPaths)
            {
                AdditionalLookupPaths[j++] = element.Name;
            }
        }

        // convention for root folder name
        public string RootFolderName { get; private set; }

        // additional folders to look for in case of distributed binaries, relative to the root folder
        public string[] AdditionalLookupPaths { get; private set; }

        // convention for assembly name which provides MEF exports
        public string MEFAssemblyNamePattern { get; private set; }

        // list of ignore paths to prevent duplicate loading of assemblies
        public string[] IgnorePathPatterns { get; private set; }
    }
}
