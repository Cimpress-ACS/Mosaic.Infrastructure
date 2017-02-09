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
	using System.Linq;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Markup;

	/// <summary>
	/// Converts multiple Boolean values into a Visibility value.
	/// </summary>
	public class MultiBooleanToVisibilityConverter : MarkupExtension, IMultiValueConverter
	{
		/// <summary>
		/// FalseEquivalent (default : Visibility.Collapsed => see Constructor)
		/// </summary>
		public Visibility FalseEquivalent { get; set; }

		public MultiBooleanToVisibilityConverter()
		{
			this.FalseEquivalent = Visibility.Collapsed;
		}

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values.Any( value => !(value is bool) )) return Visibility.Visible;

			return values.Any( value => (bool)value == false ) ? this.FalseEquivalent : Visibility.Visible;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new[] { new object() };
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}
