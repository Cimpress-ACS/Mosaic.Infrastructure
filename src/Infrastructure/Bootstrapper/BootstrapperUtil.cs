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
using System.IO;
using System.Reflection;

namespace VP.FF.PT.Common.Infrastructure.Bootstrapper
{
    public class BootstrapperUtil
    {
        /// <summary>
        /// Search for an assembly file in the given directories and their sub-directories recursively.
        /// </summary>
        /// <remarks>
        /// This helps to find .resource assemblies (localization resources) for example.
        /// </remarks>
        /// <param name="filename">Filename of the assembly to search for.</param>
        /// <param name="directories">Directories to search.</param>
        /// <returns>The loaded assembly or null if not found.;</returns>
        public static Assembly TryLoadAssembly(string filename, IEnumerable<string> directories)
        {
            foreach (string folderPath in directories)
            {
                if (folderPath == null)
                    continue;

                var files = Directory.GetFiles(folderPath, "*.dll", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.Name.Equals(filename))
                    {
                        return Assembly.LoadFrom(file);
                    }
                }
            }

            return null;
        }

        public static string FindRoot(string path, string directoryName)
        {
            var directoryInfo = new DirectoryInfo(path);
            while (!directoryInfo.Name.Equals(directoryName, StringComparison.InvariantCultureIgnoreCase))
            {
                var parent = directoryInfo.Parent;
                if (parent == null)
                {
                    throw new IOException(string.Format("Root folder '{0}' was not found, starting from '{1}'",
                        directoryName, path));
                }

                directoryInfo = parent;
            }

            return directoryInfo.FullName;
        }
    }
}
