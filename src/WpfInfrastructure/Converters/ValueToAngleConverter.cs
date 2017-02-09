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
using System.Windows.Data;

namespace VP.FF.PT.Common.WpfInfrastructure.Converters
{
    public class ValueToAngleConverter : IMultiValueConverter
    {
        private const double maxAngle = 118;

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length < 2)
            {
                return Binding.DoNothing;
            }

            double currentValue = double.Parse(values[0].ToString());
            double maxValue = double.Parse(values[1].ToString());
            return (maxAngle / maxValue) * currentValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
