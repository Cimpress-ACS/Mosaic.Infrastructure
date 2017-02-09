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
using VP.FF.PT.Common.Infrastructure.Bootstrapper.BootstrapperConfigSection;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace VP.FF.PT.Common.Infrastructure.Bootstrapper
{
    /// <summary>
    /// Loads all needed assemblies.
    /// </summary>
    /// <remarks>
    /// Works for development folder as well as for deployment folder because it is using the SafeRecursiveDirectoryCatalog
    /// and handles not resolved assemblies. So no build script or post copy step is needed.
    /// </remarks>
    public abstract class MefBootstrapper : IDisposable
    {
        private bool _disposed;
        private SafeRecursiveDirectoryCatalog _recursiveDirectoryCatalog;       
        private readonly BootrapperConfiguration _bootrapperConfiguration;

        public AggregateCatalog AdditionalCatalogs { get; private set; }

        protected MefBootstrapper()
        {
            AdditionalCatalogs = new AggregateCatalog();

            IgnorePathPatterns = new List<string>();

            _bootrapperConfiguration = new BootrapperConfiguration();

            AppDomain currentDomain = AppDomain.CurrentDomain; 
            currentDomain.AssemblyResolve += TryResolveMissingAssembly;
        }

        [Export]
        public CompositionContainer Container;

        protected ILogger Logger { get; private set; }

        protected IList<string> IgnorePathPatterns { get; private set; }

        public virtual void Run()
        {
            IList<string> ignores = _bootrapperConfiguration.IgnorePathPatterns.ToList();

            foreach (var ignorePathPattern in IgnorePathPatterns)
            {
                ignores.Add(ignorePathPattern);
            }

            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string rootPath = GetRootPath(basePath);

            var paths = new List<string> {rootPath};
            paths.AddRange(_bootrapperConfiguration.AdditionalLookupPaths.Select(additionalLookupPath => rootPath + "\\" + additionalLookupPath));

            var logger = new Log4NetLogger();
            logger.Init(GetType());
            _recursiveDirectoryCatalog = new SafeRecursiveDirectoryCatalog(
                logger,
                paths.ToArray(),
                _bootrapperConfiguration.MEFAssemblyNamePattern,
                ignores);

            BuildContainer();

            OnInitialized();

            HandleShutdownRequest();

            Logger.InfoFormat("MEF container built and initialized ({0} parts)", Container.Catalog.Parts.Count());
        }

        public virtual void Stop()
        {
            Logger.Info("Stopping Bootstrapper");

            var shutdownParts = Container.GetExportedValues<IShutdown>();

            foreach (var shutdownPart in shutdownParts)
            {
                try
                {
                    shutdownPart.Shutdown();
                }
                catch (Exception e)
                {
                    Logger.Error(e.Message, e);
                }
            }
        }

        public object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            return Container.GetExportedValue<object>(contract);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        public void BuildUp(object instance)
        {
            Container.SatisfyImportsOnce(instance);
        }

        public void LogApplicationCrash(Exception e)
        {
            Logger.Error("Application crash due to unhandled exception!", e);
        }

        /// <summary>
        /// Will be called as soon as the MEF container construction is completed.
        /// </summary>
        protected abstract void OnInitialized();

        private void BuildContainer()
        {
            var aggregateCatalog = new AggregateCatalog();

            if (Assembly.GetEntryAssembly() != null)
            {
                aggregateCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetEntryAssembly(), GlobalRegistrationBuilder.Builder));
            }
            
            aggregateCatalog.Catalogs.Add(_recursiveDirectoryCatalog);
            aggregateCatalog.Catalogs.Add(AdditionalCatalogs);

            Container = new CompositionContainer(aggregateCatalog,  CompositionOptions.DisableSilentRejection);

            var batch = new CompositionBatch();

            batch.AddExportedValue(Container);

            Container.Compose(batch);

            Logger = Container.GetExportedValue<ILogger>();
            Logger.Init(GetType());

            LogDebugInfos();
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
            var filename = args.Name.Split(',')[0] + ".dll";
            var directories =
                _recursiveDirectoryCatalog.LoadedAssemblies.Select(Path.GetDirectoryName)
                    .Where(x => x != null)
                    .Distinct();
            return BootstrapperUtil.TryLoadAssembly(filename, directories);
        }

        private void LogDebugInfos()
        {
            Logger.Info("Loaded Assemblies:");

            foreach (var assemblyPath in _recursiveDirectoryCatalog.LoadedAssemblies)
            {
                Logger.Info(assemblyPath);
            }
        }

        /// <summary>
        /// Handles the shutdown request.
        /// The application can be closed at any time using EventAggregator and the public event <see cref="ShutdownEvent"/>.
        /// </summary>
        private void HandleShutdownRequest()
        {
            var eventAggregator = Container.GetExportedValue<IEventAggregator>();
            eventAggregator.GetEvent<ShutdownEvent>().Subscribe(e =>
            {
                Logger.Info(e.Source + " requested a shutdown. ErrorCode=" + e.ErrorCode + " Reason=" + e.Reason);
                Stop();
            });
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _recursiveDirectoryCatalog.Dispose();
                Container.Dispose();

                _disposed = true;
            }
        }
    }
}
