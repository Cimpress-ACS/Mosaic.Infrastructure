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


// ReSharper disable once CheckNamespace
namespace VP.FF.PT.Common.WpfInfrastructure.Converters
{
	using System;
	using System.Globalization;
	using System.Windows.Data;
	using System.Windows.Markup;

	/// <summary>
	/// Converts a String into it's Upper Equivalent.
	/// </summary>
	[ValueConversion( typeof( string ), typeof( string ) )]
	[MarkupExtensionReturnType( typeof( StringToUpperStringConverter ) )]
	public class StringToUpperStringConverter : MarkupExtension, IValueConverter
	{
		public StringToUpperStringConverter() { }

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string) return value.ToString().ToUpper();

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// No suitable implementation possible !
			throw new NotSupportedException();
		}

		#endregion

		#region MarkupExtension "overrides"

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		#endregion
	}
}
