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


using System.Windows;
using System.Windows.Controls.Primitives;

namespace VP.FF.PT.Common.WpfInfrastructure.Controls
{
    public class ToggleBusyButton : ToggleButton
    {        
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBusy.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool), typeof(ToggleBusyButton), new PropertyMetadata(false));


        static ToggleBusyButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleBusyButton), new FrameworkPropertyMetadata(typeof(ToggleBusyButton)));
        }
    }
}
