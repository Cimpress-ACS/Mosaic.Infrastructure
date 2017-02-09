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

namespace VP.FF.PT.Common.WpfInfrastructure.Controls
{
    public class FeedbackDialog : ContentControl
    {
        public static readonly DependencyProperty DialogHeightProperty = DependencyProperty.Register(
            "DialogHeight", typeof(double), typeof(FeedbackDialog), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty DialogWidthProperty = DependencyProperty.Register(
            "DialogWidth", typeof(double), typeof(FeedbackDialog), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register(
            "TitleText", typeof(string), typeof(FeedbackDialog), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty ContentSectionProperty = DependencyProperty.Register(
            "ContentSection", typeof(object), typeof(FeedbackDialog), new PropertyMetadata(default(object)));

        public static readonly DependencyProperty ActionSectionProperty = DependencyProperty.Register(
            "ActionSection", typeof(object), typeof(FeedbackDialog), new PropertyMetadata(default(object)));

        public double DialogHeight
        {
            get { return (double)GetValue(DialogHeightProperty); }
            set { SetValue(DialogHeightProperty, value); }
        }

        public double DialogWidth
        {
            get { return (double)GetValue(DialogWidthProperty); }
            set { SetValue(DialogWidthProperty, value); }
        }

        public string TitleText
        {
            get { return (string) GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        public object ContentSection
        {
            get { return GetValue(ContentSectionProperty); }
            set { SetValue(ContentSectionProperty, value); }
        }

        public object ActionSection
        {
            get { return GetValue(ActionSectionProperty); }
            set { SetValue(ActionSectionProperty, value); }
        }
    }
}
