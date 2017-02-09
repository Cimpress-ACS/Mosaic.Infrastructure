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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VP.FF.PT.Common.WpfInfrastructure.Converters
{
    using System.Windows.Data;

    public class ShirtTypeToHeadStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null)
            {
                return Binding.DoNothing;
            }

            var targetElement = values[0] as FrameworkElement;
            var styleName = values[1] as string; 
            if (styleName == null)
            {
                return Binding.DoNothing;
            }

            var headType = "";

            if (styleName.Equals("WomensTShirt"))
            {
                headType = "Female";
            }
            else if (styleName.Equals("YouthTShirt"))
            {
                headType = "Youth";
            }
            else
            {
                headType = "Male";
            }

            var newStyleName = "Shirt.Head." + headType + "." + parameter.ToString();

            //Shirt.Head.Youth

            //var newStyleName = "Shirt.Type.YouthTShirt." + parameter.ToString();


            //Shirt.Type.YouthTShirt.
            //Shirt.Type.LongSleeveShirt.
            //Shirt.Type.PremiumTShirt.
            //Shirt.Type.YouthTShirt
            //Shirt.Type.WomensTShirt.

            if (newStyleName == null)
                return null;

            var newStyle = (Style)targetElement.TryFindResource(newStyleName);

            return newStyle;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
