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


using Moq;
using NUnit.Framework;
using VP.FF.PT.Common.TestInfrastructure;
using VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration;
using VP.FF.PT.Common.WpfInfrastructure.ScreenActivation;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Services;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.UnitTests.Screens.ViewModels
{
    public class GenericPlcViewModelTests_Base
    {
        protected const string Loa1 = "LOA1";

        protected string _randomModule;

        protected Mock<IProvidePlcInformation> _informationProvider;
        protected Mock<IProvideStatesForScreenActivation> _states;
        protected Mock<IProvideStatesForScreenActivation> _detailStates;
        protected Mock<ISaveToFileSystem> _saver;
        protected Mock<ILoadFromFileSystem> _loader;
        protected Mock<IAskUser> _askUser;

        [SetUp]
        public void Setup()
        {
            _randomModule = CreateRandom.String();
            _informationProvider = new Mock<IProvidePlcInformation>();
            _states = new Mock<IProvideStatesForScreenActivation>();
            _detailStates = new Mock<IProvideStatesForScreenActivation>();
            _saver = new Mock<ISaveToFileSystem>();
            _loader = new Mock<ILoadFromFileSystem>();
            _askUser = new Mock<IAskUser>();
            SetupConcreteContext();
        }

        protected virtual void SetupConcreteContext()
        {
        }

        protected GenericPlcViewModel CreateGenericPlcViewModel(string moduleName = Loa1)
        {
            return new GenericPlcViewModel(
                _informationProvider.Object, 
                null, 
                _states.Object,
                _detailStates.Object,
                _saver.Object, 
                _loader.Object, 
                _askUser.Object,
                null, 
                moduleName, 
                new NonDispatchingDispatcher());
        }

        protected class TestableGenericPlcViewModel : GenericPlcViewModel
        {
            public TestableGenericPlcViewModel(
                IProvidePlcInformation informationProvider, 
                ICommandControllers controllerCommander, 
                IProvideStatesForScreenActivation states,
                IProvideStatesForScreenActivation detailStates,
                ISaveToFileSystem saver,
                ILoadFromFileSystem loader,
                IAskUser askUser,
                IModuleScreen parent, 
                string moduleName) 
                : base(informationProvider, controllerCommander, states, detailStates, saver, loader, askUser, parent, moduleName, new NonDispatchingDispatcher())
            {
            }

            public void Activate()
            {
                OnActivate();
            }

            public void Deactivate(bool close = false)
            {
                OnDeactivate(close);
            }
        }

        protected TestableGenericPlcViewModel CreateTestableGenericPlcViewModel(string moduleName = "LOA1")
        {
            return new TestableGenericPlcViewModel(
                _informationProvider.Object, 
                null, 
                _states.Object,
                _detailStates.Object, 
                _saver.Object, 
                _loader.Object, 
                _askUser.Object,
                null, 
                moduleName);
        }

        protected MasterDetailViewModel MasterDetailViewModel(string moduleName, int controllerId)
        {
            return new MasterDetailViewModel(new Controller{Id = controllerId}, moduleName, new ControllerCommanderNullObject(), _states.Object);
        }
    }
}
