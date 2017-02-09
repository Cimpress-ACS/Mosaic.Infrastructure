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
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.Bootstrapper;
using VP.FF.PT.Common.Infrastructure.Database;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.Infrastructure.UnitTests.Properties;

namespace VP.FF.PT.Common.Infrastructure.UnitTests.Bootstrapping
{
    [TestFixture]
    public class RegistrationBuilderTest
    {
        static RegistrationBuilderTest()
        {
            GlobalRegistrationBuilder.Builder
                .ForTypesDerivedFrom<ITestInterfaceWithoutAttributes>()
                .Export<ITestInterfaceWithoutAttributes>();

            if (Settings.Default.IsSimulation)
            {
                GlobalRegistrationBuilder.Builder
                    .ForType<ConsumeTestPart>()
                    .ImportProperty(p => p.ImportedTestPart, builder => builder.AsContractName("simulation"));
            }
            else
            {
                GlobalRegistrationBuilder.Builder
                    .ForTypesMatching(type => type.GetProperties().Any(p => p.PropertyType == typeof(ITestPart)))
                    .ImportProperties<ITestPart>(_ => true);
            }
        }

        private MefBootstrapper _bootstrapper;

        [SetUp]
        public void SetUp()
        {
            _bootstrapper = new TestBootstrapper();
        }

        [Test]
        public void BuildContainerMustNotThrow()
        {
            var runAction = new Action(() => _bootstrapper.Run());

            runAction.ShouldNotThrow();

            var stopAction = new Action(() => _bootstrapper.Stop());

            stopAction.ShouldNotThrow();
        }

        [Test]
        public void TestAttributelessRegistrationUsingGlobalRegistrationBuilder()
        {
            _bootstrapper.Run();

            var result = _bootstrapper.Container.GetExportedValueOrDefault<ITestInterfaceWithoutAttributes>();
            result.Should().BeOfType<TestClassWithoutAttributes>();
        }

        [Test]
        public void OverridePartByASimulatingPart()
        {
            _bootstrapper.Run();

            var consumerOfTestPart = _bootstrapper.Container.GetExportedValue<ConsumeTestPart>();

            consumerOfTestPart.ImportedTestPart.Should().BeOfType<TestSimulationPart>("the convention rule in the static ctor defined it like this (using the global RegistrationBuilder)");
        }
    }

    public interface ITestInterfaceWithoutAttributes
    {
    }

    public class TestClassWithoutAttributes : ITestInterfaceWithoutAttributes
    {
    }

    public interface ITestPart
    {
    }
    
    [Export(typeof(ITestPart))]
    public class TestPartA : ITestPart
    {
        [ImportingConstructor]
        public TestPartA(ILogger logger)
        {}
    }

    [Export("simulation", typeof(ITestPart))]
    public class TestSimulationPart : ITestPart
    {
        [ImportingConstructor]
        public TestSimulationPart(ILogger logger)
        {}
    }

    [Export]
    public class ConsumeTestPart
    {
        public ITestPart ImportedTestPart { get; set; }

        [ImportingConstructor]
        public ConsumeTestPart(ILogger someLogger)
        {
        }
    }

    // TODO: move this to infrastructure and create an ExportProvider to export it as default if no other export is there
    // TODO: do the same for IModuleBusManager, IJobManager
    [Export(typeof(IConnectionStringProvider))]
    public class DefaultConnectionStringProvider : IConnectionStringProvider
    {
        [ImportingConstructor]
        public DefaultConnectionStringProvider(ILogger logger)
        {
            logger.Init(GetType());
            logger.Info("Couldn't found any IConnectionStringProvider in MEF container. Default will be used.");

            ConnectionString = string.Empty;
        }

        public string ConnectionString { get; set; }
    }
}
