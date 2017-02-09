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
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace VP.FF.PT.Common.Infrastructure.Bootstrapper
{
    public class SafeRecursiveDirectoryCatalog : ComposablePartCatalog
    {
        private readonly AggregateCatalog _catalog;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeRecursiveDirectoryCatalog"/> class.
        /// Searches all subfolders recursively for MEF exports and serves as an AggregateCatalog.
        /// Further the directory will skip all "bad" assemblies and no exception will be thrown.
        /// The search starts in the directory where the executing assembly is located (this prevents ambiguous problems and CompositionException).
        /// </summary>
        /// <param name="directories">The root directories to search for assemblies.</param>
        /// <param name="searchPattern">The search pattern. Wildcard is possible. Multiple search patterns are supported, separated by ;</param>
        /// <param name="skipPatterns">The skip patterns to skip paths containing the specified substring.</param>
        public SafeRecursiveDirectoryCatalog(ILogger logger, string[] directories, string searchPattern = "*.dll", IEnumerable<string> skipPatterns = null)
        {
            _logger = logger;
            LoadedAssemblies = new List<string>();
            _catalog = new AggregateCatalog();

            // first search in current directory
            DirectoryInfo currentDirectory = Directory.GetParent(Assembly.GetExecutingAssembly().GetName().CodeBase.Replace("file:///", string.Empty));

            var patterns = searchPattern.Split(';');
            List<string> files = new List<string>();
            foreach (var pattern in patterns)
            {
                files.AddRange(Directory.EnumerateFiles(currentDirectory.FullName, pattern.Trim(), SearchOption.AllDirectories));
            }
                
            LoadAssemblies(files, skipPatterns);

            foreach (var directory in directories)
            {
                if (!Directory.Exists(directory))
                {
                    _logger.WarnFormat("Cannot import assemblies from {0} because directory does not exist!", directory);
                    continue;
                }

                // search all other directories
                List<string> additionalFiles = new List<string>();
                foreach (var pattern in patterns)
                {
                    additionalFiles.AddRange(Directory.EnumerateFiles(directory, pattern, SearchOption.AllDirectories));
                }

                additionalFiles = additionalFiles.Except(files).ToList();
                LoadAssemblies(additionalFiles, skipPatterns);
            }
        }

        private void LoadAssemblies(IEnumerable<string> paths, IEnumerable<string> skipPatterns)
        {
            foreach (var path in paths)
            {
                if (skipPatterns != null && CheckPattern(skipPatterns, path))
                    continue;

                try
                {
                    var asmCat = new AssemblyCatalog(path, GlobalRegistrationBuilder.Builder);

                    // Force MEF to load the plugin and figure out if there are any exports
                    // good assemblies will not throw the RTLE exception and can be added to the catalog
                    if (asmCat.Parts.Any())
                    {
                        if (CheckAlreadyLoaded(path))
                            continue;

                        _catalog.Catalogs.Add(asmCat);
                        LoadedAssemblies.Add(path);
                    }
                }
                catch (ReflectionTypeLoadException e)
                {
                    var typesStringBuilder = new StringBuilder();
                    foreach (var type in e.Types)
                    {
                        typesStringBuilder.Append(type);
                    }

                    _logger.WarnFormat("Could not load assembly '{0}' . Depending types: {1}", e, typesStringBuilder);

                    foreach (var ex in e.LoaderExceptions)
                    {
                        _logger.WarnFormat("LoaderException: '{0}'", ex);
                    }
                    
                }
            }
        }

        private bool CheckPattern(IEnumerable<string> skipPatterns, string path)
        {
            if (skipPatterns == null)
                return false;

            if (skipPatterns.Any(path.Contains))
            {
                return true;
            }

            return false;
        }

        private bool CheckAlreadyLoaded(string file)
        {
            foreach (var loadedAssembly in LoadedAssemblies)
            {
                if (loadedAssembly.Split('\\').Last().Equals(file.Split('\\').Last()))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the loaded assemblies.
        /// </summary>
        public IList<string> LoadedAssemblies { get; private set; }

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get { return _catalog.Parts; }
        }
    }

}
