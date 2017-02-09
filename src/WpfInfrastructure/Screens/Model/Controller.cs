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
using System.Collections.ObjectModel;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Model
{
    public class Controller
    {
        public Controller()
        {
            Children = new Controller[0];
        }

        public string ActiveAlarm { get; set; }

        public IEnumerable<Tag> ActualValues { get; set; }

        public IEnumerable<Controller> Children { get; set; }

        public IEnumerable<Tag> Configurations { get; set; }

        public ControllerState ControllerState { get; set; }

        public ObservableCollection<CommandViewModel> Commands { get; set; }

        public string CurrentState { get; set; }

        public string CurrentSubState { get; set; }

        public bool EnableForcing { get; set; }

        public int Id { get; set; }

        public IEnumerable<Tag> Inputs { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsSimulation { get; set; }

        public Mode ControllerMode { get; set; }

        public string Name { get; set; }

        public string PlcControllerPath { get; set; }

        public string FullName { get; set; }

        public IEnumerable<Tag> Outputs { get; set; }

        public IEnumerable<Tag> Parameters { get; set; }

        public string Type { get; set; }

        public enum Mode
        {
            Auto = 10,
            Manual = 12
        }
    }
}
