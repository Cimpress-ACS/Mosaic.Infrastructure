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


using System.ComponentModel.Composition;
using Caliburn.Micro;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class MessageBoxViewModel : Screen
    {
        private string _title;
        private string _message;
        private double _scalingFactor;

        public MessageBoxViewModel(double scalingFactor, string title, string message)
        {
            _title = title;
            _message = message;
            _scalingFactor = scalingFactor;
        }

        public void Close()
        {
            TryClose();
        }

        public double ScalingFactor
        {
            get { return _scalingFactor; }

            set
            {
                if (value != _scalingFactor)
                {
                    _scalingFactor = value;
                    NotifyOfPropertyChange(() => ScalingFactor);
                }
            }
        }

        public string Title
        {
            get { return _title; }

            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyOfPropertyChange(() => Title);
                }
            }
        }

        public string Message
        {
            get { return _message; }

            set
            {
                if (value != _message)
                {
                    _message = value;
                    NotifyOfPropertyChange(() => Message);
                }
            }
        }
    }
}
