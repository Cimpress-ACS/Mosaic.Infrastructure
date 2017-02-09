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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace LoggingSampleApp
{
    class Program
    {
        private static CompositionContainer _container;

        static void Main(string[] args)
        {
            //An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();
            //Adds all the parts found in the same assembly as the Program class
            //catalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ILogger).Assembly));

            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog);

            //Fill the imports of this object
            try
            {
                Console.WriteLine("Testing application for log messages");
                _container.ComposeParts();

                ILogger logger = _container.GetExportedValue<ILogger>();

                Console.WriteLine("Obtained logger '{0}'", logger.LoggerName);

                while (true)
                {
                    Console.WriteLine("Enter a message to be logged (Q to quit)");
                    string msg = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(msg) || msg.ToLower() == "q") break;
                    logger.DebugFormat("Debug message: {0}", msg);
                }
            }
            catch (CompositionException compositionException)
            {
                Console.WriteLine(compositionException.ToString());
            }
        }
    }
}
