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
using System.Windows.Data;
using System.Windows.Markup;

namespace VP.FF.PT.Common.WpfInfrastructure.Converters
{
	/// <summary>
	/// 
	/// </summary>
	[ValueConversion( typeof( string ), typeof( string ) )]
	[MarkupExtensionReturnType( typeof( ToUpperStringConverter ) )]
	public class ToUpperStringConverter : MarkupExtension, IValueConverter
	{
		public ToUpperStringConverter() { }

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is string)) return Binding.DoNothing;

			var str = value as string;
			return string.IsNullOrEmpty( str ) ? string.Empty : str.ToUpper();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		#endregion // IValueConverter Members

		#region MarkupExtension "overrides"

		/// <summary>
		/// Overrides the abstract "ProvideValue" method from the "MarkupExtension" class
		/// </summary>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return new ToUpperStringConverter();
		}

		#endregion
	}
}
