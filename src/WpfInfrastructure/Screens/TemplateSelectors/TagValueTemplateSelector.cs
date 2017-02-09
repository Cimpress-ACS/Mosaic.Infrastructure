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
using System.Windows;
using System.Windows.Controls;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.TemplateSelectors
{
    /// <summary>
    /// The <see cref="TagValueTemplateSelector"/> is able determine which <see cref="UserControl"/> instance
    /// is needed to display a <see cref="KeyValueUnitViewModel"/> instance.
    /// </summary>
    public class TagValueTemplateSelector : DataTemplateSelector
    {
        public DataTemplate BooleanValue { get; set; }
        public DataTemplate StringValue { get; set; }
        public DataTemplate IntValue { get; set; }
        public DataTemplate ReadOnlyValue { get; set; }
        public DataTemplate EnumerationValue { get; set; }

        /// <summary>
        /// When overridden in a derived class, returns a <see cref="T:System.Windows.DataTemplate"/> based on custom logic.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="T:System.Windows.DataTemplate"/> or null. The default value is null.
        /// </returns>
        /// <param name="item">The data object for which to select the template.</param><param name="container">The data-bound object.</param>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var viewModel = item as KeyValueUnitViewModel;
            if (viewModel == null)
                return null;
            if (!viewModel.EnumerationMembers.IsNullOrEmpty())
                return EnumerationValue;
            if (viewModel.Type == null || viewModel.Type == typeof(Array))
                return CustomValue();
            if (viewModel.IsReadOnly)
                return ReadOnlyValue;
            if (viewModel.Type == typeof (bool))
                return BooleanValue;
            if (IsNumberDataType(viewModel.Type))
                return IntValue;
            if (viewModel.Type == typeof (string))
                return StringValue;
            return ReadOnlyValue;
        }

        private bool IsNumberDataType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        private DataTemplate CustomValue()
        {
            return Application.Current.MainWindow.FindResource("CustomValue") as DataTemplate;
        }
    }
}
