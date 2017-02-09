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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using VP.FF.PT.Common.Infrastructure.Bootstrapper;
using VP.FF.PT.Common.Infrastructure.Bootstrapper.BootstrapperConfigSection;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration
{
    /// <summary>
    /// Loads all needed assemblies.
    /// </summary>
    /// <remarks>
    /// Works for development folder as well as for deployment folder because it is using the SafeRecursiveDirectoryCatalog
    /// and handles not resolved assemblies. So no build script or post copy step is needed.
    /// </remarks>
    public class MefBootstrapper : Bootstrapper<IShell>
    {
        private CompositionContainer _container;
        private SafeRecursiveDirectoryCatalog _recursiveDirectoryCatalog;

        private BootrapperConfiguration _bootrapperConfiguration;

        static MefBootstrapper()
        {
            LogManager.GetLog = type => new Log4NetAdapter(new Log4NetLogger()).Init(type);
        }

        public MefBootstrapper()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain; 
            currentDomain.AssemblyResolve += TryResolveMissingAssembly;  
        }

        protected override void Configure()
        {
            var aggregateCatalog = new AggregateCatalog();
            aggregateCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetEntryAssembly()));
            aggregateCatalog.Catalogs.Add(_recursiveDirectoryCatalog);

            _container = new CompositionContainer(aggregateCatalog);

            var batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(_container);

            _container.Compose(batch);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            _bootrapperConfiguration = new BootrapperConfiguration();

            if (DesignTimeHelper.IsInDesignModeStatic)
                return null;

            var paths = new List<string>();

            // collect all assemblies with catalog and safe it for later use
            var uri = new Uri(Assembly.GetAssembly(typeof(MefBootstrapper)).CodeBase);
            string rootPath = GetRootPath(uri.LocalPath);

            paths.Add(rootPath);
            foreach (var additionalLookupPath in _bootrapperConfiguration.AdditionalLookupPaths)
                paths.Add(rootPath + "\\" + additionalLookupPath);

            var logger = new Log4NetLogger();
            logger.Init(GetType());
            _recursiveDirectoryCatalog = new SafeRecursiveDirectoryCatalog(
                logger,
                paths.ToArray(), 
                _bootrapperConfiguration.MEFAssemblyNamePattern, 
                _bootrapperConfiguration.IgnorePathPatterns);

            IList<Assembly> list = new List<Assembly>();
            list.Add(Assembly.GetEntryAssembly());

            foreach(string assemblyPath in _recursiveDirectoryCatalog.LoadedAssemblies)
                list.Add(Assembly.LoadFrom(assemblyPath));

            return list;
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            return _container.GetExportedValue<object>(contract);
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }

        private string GetRootPath(string searchFromPath)
        {
            return BootstrapperUtil.FindRoot(searchFromPath, _bootrapperConfiguration.RootFolderName);
        }

        /// <summary>
        /// Help .NET resolving missing assemblies (because we distribute assemblies in different directories, e.g. for each MEF module).
        /// Include search in same folders where MEF already loads some parts.
        /// This method will search recursively in all subdirectories as well.
        /// </summary>
        /// <remarks>
        /// This helps to find .resource assemblies (localization resources) for example.
        /// </remarks>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.ResolveEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private Assembly TryResolveMissingAssembly(object sender, ResolveEventArgs args)
        {
            if (DesignTimeHelper.IsInDesignModeStatic)
                return null;

            var filename = args.Name.Split(',')[0] + ".dll";
            var directories =
                _recursiveDirectoryCatalog.LoadedAssemblies.Select(Path.GetDirectoryName)
                    .Where(x => x != null)
                    .Distinct();
            return BootstrapperUtil.TryLoadAssembly(filename, directories);
        }
    }
}
    
