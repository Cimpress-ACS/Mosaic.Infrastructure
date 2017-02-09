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
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Markup;
	using System.Windows.Media;

	/// <summary>
	/// Converts a Boolean value into a Brush (and back).
	/// The Brushes for the true and the false value can be declared separately.
	/// </summary>
	[ValueConversion( typeof( bool ), typeof( Brush ) )]
	[MarkupExtensionReturnType( typeof( BoolToBrushConverter ) )]
	public class BoolToBrushConverter : MarkupExtension, IValueConverter
	{
		/// <summary>
		/// TrueValueBrush (default : MediumSpringGreen => see Constructor).
		/// </summary>
		public Brush TrueValueBrush { get; set; }

		/// <summary>
		/// FalseValueBrush (default : White => see Constructor).
		/// </summary>
		public Brush FalseValueBrush { get; set; }

		/// <summary>
		/// Initialize the 'True' and 'False' Brushes with standard values.
		/// </summary>
		public BoolToBrushConverter()
		{
			TrueValueBrush = Brushes.MediumSpringGreen;
			FalseValueBrush = Brushes.White;
		}

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
			{
				return (bool)value ? TrueValueBrush : FalseValueBrush;
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter,
										  CultureInfo culture)
		{
			if (value is Brush)
			{
				return value.Equals( TrueValueBrush );
			}

			return value;
		}

		#endregion // IValueConverter Members

		#region MarkupExtension "overrides"

		/// <summary>
		/// Overrides the abstract "ProvideValue" method from the "MarkupExtension" class
		/// </summary>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		#endregion
	}

	/// <summary>
	/// Converts a Boolean value into an opposite Boolean value (and back).
	/// </summary>
	[ValueConversion( typeof( bool ), typeof( bool ) )]
	[MarkupExtensionReturnType( typeof( BoolToOppositeBoolConverter ) )]
	public class BoolToOppositeBoolConverter : MarkupExtension, IValueConverter
	{
		public BoolToOppositeBoolConverter() { }

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
			{
				return !(bool)value;
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool)
			{
				return !(bool)value;
			}

			return value;
		}

		#endregion

		#region MarkupExtension "overrides"

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceProvider"></param>
		/// <returns></returns>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		#endregion
	}

	/// <summary>
	/// Converts a Boolean value into a Visibility enumeration (and back).
	/// </summary>
	[ValueConversion( typeof( bool ), typeof( Visibility ) )]
	[MarkupExtensionReturnType( typeof( BoolToVisibilityConverter ) )]
	public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
	{
		/// <summary>
		/// FalseEquivalent (default : Visibility.Collapsed => see Constructor).
		/// </summary>
		public Visibility FalseEquivalent { get; set; }
		/// <summary>
		/// Define whether the opposite boolean value is crucial (default : false).
		/// </summary>
		public bool OppositeBooleanValue { get; set; }

		/// <summary>
		/// Initialize the properties with standard values.
		/// </summary>
		public BoolToVisibilityConverter()
		{
			this.FalseEquivalent = Visibility.Collapsed;
			this.OppositeBooleanValue = false;
		}

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool && targetType == typeof( Visibility ))
			{
				bool? booleanValue = (bool?)value;

				if (this.OppositeBooleanValue)
				{
					booleanValue = !booleanValue;
				}

				return booleanValue.GetValueOrDefault() ? Visibility.Visible : FalseEquivalent;
			}

			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Visibility)
			{
				Visibility visibilityValue = (Visibility)value;

				if (this.OppositeBooleanValue == true)
				{
					visibilityValue = visibilityValue == Visibility.Visible ? FalseEquivalent : Visibility.Visible;
				}

				return (visibilityValue == Visibility.Visible);
			}

			return value;
		}

		#endregion // IValueConverter Members

		#region MarkupExtension "overrides"

		/// <summary>
		/// 
		/// </summary>
		/// <param name="serviceProvider"></param>
		/// <returns></returns>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}

		#endregion
	}

	/// <summary>
	/// Class BoolToIndexConverter.
	/// </summary>
	[ValueConversion( typeof( bool ), typeof( int ) )]
	[MarkupExtensionReturnType( typeof( BoolToIndexConverter ) )]
	public class BoolToIndexConverter : MarkupExtension, IValueConverter
	{
		public BoolToIndexConverter() { }

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is bool)) return 0;

			return (bool)value ? 0 : 1;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((int)value == 0);
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
