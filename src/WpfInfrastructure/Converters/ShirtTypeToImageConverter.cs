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
using System.IO;
using System.Windows.Data;

namespace VP.FF.PT.Common.WpfInfrastructure.Converters
{
    using VP.FF.PT.Common.WpfInfrastructure.Extensions;

    public class ShirtTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = (string)value;

            string iconKey;
            
            // find images for shirt type
            if (parameter != null && parameter.ToString().Equals("POI"))
            {
                iconKey = "underlay_POI_" + type.ToLower();
            }
            else if (parameter != null && parameter.ToString().Equals("Lines"))
            {
                iconKey = "shirt_" + type.ToLower() + "_line";
            }
            else
            {
                iconKey = "shirt_" + type.ToLower();
            }

            var imgUri = new Uri(LoadImageHelper.GetIconPathOrFallback(Directory.GetCurrentDirectory() + "/Images/", iconKey, "shirt_missing_image"), UriKind.Absolute);
        
            return imgUri;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
