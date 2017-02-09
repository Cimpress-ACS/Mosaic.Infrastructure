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
using Moq;
using NUnit.Framework;
using VP.FF.PT.Common.TestInfrastructure;
using VP.FF.PT.Common.WpfInfrastructure.ScreenActivation;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Services;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.UnitTests.Screens.ViewModels
{
    public class MasterDetailViewModelTests_Base
    {
        protected string _randomModule;
        protected int _randomController;
        protected Controller _innerController;

        protected Mock<ICommandControllers> _commander;
        protected Mock<IProvideStatesForScreenActivation> _states;

        [SetUp]
        public void Setup()
        {
            _randomModule = CreateRandom.String();
            _randomController = CreateRandom.Int();
            _innerController = new Controller();

            _commander = new Mock<ICommandControllers>();
            _states = new Mock<IProvideStatesForScreenActivation>();

            SetupConcrete();
        }

        protected virtual void SetupConcrete()
        {
        }

        protected MasterDetailViewModel CreateMasterDetailViewModel()
        {
            return CreateMasterDetailViewModel(_randomModule, _randomController);
        }

        protected MasterDetailViewModel CreateMasterDetailViewModel(string moduleName, int controllerId)
        {
            _innerController.Id = controllerId;
            return CreateMasterDetailViewModel(_innerController, moduleName);
        }

        protected MasterDetailViewModel CreateMasterDetailViewModel(Controller controller)
        {
            return CreateMasterDetailViewModel(controller, _randomModule);
        }

        protected MasterDetailViewModel CreateMasterDetailViewModel(Controller controller, string moduleName)
        {
            return new MasterDetailViewModel(controller, moduleName, _commander.Object, _states.Object);
        }

        protected KeyValueChange RandomKeyValueChange()
        {
            string randomName = CreateRandom.String();
            return new KeyValueChange(string.Empty, randomName, 1, 2, typeof (Int32));
        }
    }
}
