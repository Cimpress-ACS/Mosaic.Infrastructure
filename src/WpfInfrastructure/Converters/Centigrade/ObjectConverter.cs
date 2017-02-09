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

namespace VP.FF.PT.Common.WpfInfrastructure.Converters.Centigrade
{
	/// <summary>
	/// Converts an Type into a Boolean (and back). If the type is String the conversion will return
	/// true. Otherwise the conversion will return false.
	/// </summary>
	[ValueConversion( typeof( object ), typeof( bool ) )]
	[MarkupExtensionReturnType( typeof( StringTypeToBooleanConverter ) )]
	public class StringTypeToBooleanConverter : MarkupExtension, IValueConverter
	{
		public bool OppositeBooleanValue { get; set; }

		public StringTypeToBooleanConverter()
		{
			this.OppositeBooleanValue = false;
		}

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
            var type = value as Type;
            if (type != null)
            {
                return type == typeof(string);
            }
            return false;
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

    /// <summary>
	/// Converts an Object into a Boolean (and back). If the object is the conversion will return
	/// true. Otherwise the conversion will return false.
	/// </summary>
	[ValueConversion( typeof( object ), typeof( bool ) )]
	[MarkupExtensionReturnType( typeof( StringTypeToBooleanConverter ) )]
	public class StringToBooleanConverter : MarkupExtension, IValueConverter
	{
		public bool OppositeBooleanValue { get; set; }

		public StringToBooleanConverter()
		{
			this.OppositeBooleanValue = false;
		}

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
            return this.OppositeBooleanValue ? !(value is string) : value is string;
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
