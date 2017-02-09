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
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VP.FF.PT.Common.WpfInfrastructure.Converters
{
    public class IconKeyToIconStyleMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var returnVal = DependencyProperty.UnsetValue;

            if (values != null)
            {
                var targetElement = values[0] as FrameworkElement;
                var iconKey = values[1] as string;

                if ((targetElement == null) || (iconKey == null))
                {
                    throw new ArgumentException("MultiConverter expects targetElement as first and iconKey as second argument.");
                }

                var resourceType = parameter as string ?? string.Empty;
                var resourceStyleName = "Icon." + resourceType + iconKey;

                returnVal = targetElement.TryFindResource(resourceStyleName) as Style;
            }
            return returnVal;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
