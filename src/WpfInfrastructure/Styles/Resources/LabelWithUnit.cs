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

namespace VP.FF.PT.Common.WpfInfrastructure.Styles.Resources
{
    /// <summary>
    /// The <see cref="LabelWithUnit"/> is a label control
    /// with an assignable unit representation on the end.
    /// </summary>
    public class LabelWithUnit : Label
    {
        public static readonly DependencyProperty UnitProperty = DependencyProperty.Register(
            "Unit", typeof (object), typeof (LabelWithUnit), new PropertyMetadata(default(object)));

        public object Unit
        {
            get { return (object) GetValue(UnitProperty); }
            set { SetValue(UnitProperty, value); }
        }
    }
}
