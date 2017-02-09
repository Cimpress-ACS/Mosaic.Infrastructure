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
using System.Windows.Markup;

namespace VP.FF.PT.Common.WpfInfrastructure.Converters
{
	[ValueConversion( typeof( bool ), typeof( Visibility ) )]
	[MarkupExtensionReturnType( typeof( BooleanToVisibilityConverter ) )]
	public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
	{
		public Visibility TrueValue { get; set; }
		public Visibility FalseValue { get; set; }

		public BooleanToVisibilityConverter()
		{
			// set defaults
			TrueValue = Visibility.Visible;
			FalseValue = Visibility.Collapsed;
		}

		public object Convert(object value, Type targetType,
			 object parameter, CultureInfo culture)
		{
			if (!(value is bool))
				return null;
			return (bool)value ? TrueValue : FalseValue;
		}

		public object ConvertBack(object value, Type targetType,
			 object parameter, CultureInfo culture)
		{
			if (Equals( value, TrueValue ))
				return true;
			if (Equals( value, FalseValue ))
				return false;
			return null;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new BooleanToVisibilityConverter { FalseValue = this.FalseValue, TrueValue = this.TrueValue };
		}
    }
}
