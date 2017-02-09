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
using System.Windows.Controls;
using VP.FF.PT.Common.WpfInfrastructure.ScreenActivation;

namespace VP.FF.PT.Common.WpfInfrastructure.Controls
{
    public class DelayedLoadedContentControl : ContentControl
    {
        public static readonly DependencyProperty StatesProperty = DependencyProperty.Register(
            "States", typeof(IProvideStatesForScreenActivation), typeof(DelayedLoadedContentControl), new PropertyMetadata(default(IProvideStatesForScreenActivation), (d, e) => { }));

        public static readonly DependencyProperty ErrorActionSectionProperty = DependencyProperty.Register(
            "ErrorActionSection", typeof (object), typeof (DelayedLoadedContentControl), new PropertyMetadata(default(object), (d,e) => { }));

        public static readonly DependencyProperty LoadingScreenProperty = DependencyProperty.Register(
            "LoadingScreen", typeof (object), typeof (DelayedLoadedContentControl), new PropertyMetadata(default(object)));

        public IProvideStatesForScreenActivation States
        {
            get { return (IProvideStatesForScreenActivation)GetValue(StatesProperty); }
            set { SetValue(StatesProperty, value); }
        }

        public object ErrorActionSection
        {
            get { return (object) GetValue(ErrorActionSectionProperty); }
            set { SetValue(ErrorActionSectionProperty, value); }
        }

        public object LoadingScreen
        {
            get { return (object) GetValue(LoadingScreenProperty); }
            set { SetValue(LoadingScreenProperty, value); }
        }
    }
}
