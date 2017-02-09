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
using System.Windows.Data;

namespace VP.FF.PT.Common.WpfInfrastructure.Converters
{
    public class ShirtSizeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // TODO: read this from configuration?
            double size = (double)value;
            if (size.Equals(0.4))
            {
                return "XS";
            }

            if (size.Equals(0.5))
            {
                return "S";
            }

            if (size.Equals(0.6))
            {
                return "M";
            }

            if (size.Equals(0.7))
            {
                return "L";
            }

            if (size.Equals(0.8))
            {
                return "XL";
            }

            if (size.Equals(0.9))
            {
                return "XXL";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
