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


using Caliburn.Micro;
using VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Model
{
    public class BreadcrumbBarItem : PropertyChangedBase
    {
        public BreadcrumbBarItem(string displayName)
        {
            _displayName = displayName;
        }

        public BreadcrumbBarItem(IModuleScreen screen)
        {
            if (screen != null)
            {
                _displayName = screen.DisplayName;
                _relatedScreen = screen;
                var baseViewModel = screen as BaseViewModel;
                if (baseViewModel != null)
                {
                    _iconKey = baseViewModel.IconKey;
                }
                else
                {
                    IconKey = string.Empty;
                }
            }
        }

        private string _displayName;

        public string DisplayName
        {
            get { return _displayName; }

            set
            {
                if (value != _displayName)
                {
                    _displayName = value;
                    NotifyOfPropertyChange(() => DisplayName);
                }
            }
        }

        private string _iconKey;

        public string IconKey
        {
            get { return _iconKey; }

            set
            {
                if (value != _iconKey)
                {
                    _iconKey = value;
                    NotifyOfPropertyChange(() => IconKey);
                }
            }
        }

        private IScreen _relatedScreen;

        public IScreen RelatedScreen
        {
            get { return _relatedScreen; }

            set
            {
                if (_relatedScreen != value)
                {
                    _relatedScreen = value;
                    NotifyOfPropertyChange(() => RelatedScreen);
                }
            }
        }

    }

}
