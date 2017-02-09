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
    /// Converts a Double value range into a Boolean (and back).
    /// </summary>
    [ValueConversion(typeof(double), typeof(bool))]
    [MarkupExtensionReturnType(typeof(DoubleLimitToBooleanConverter))]
    public class DoubleLimitToBooleanConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Limit (default : 0 => see Constructor).
        /// </summary>
        public double Limit { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleLimitToBooleanConverter"/> class. Initialize all Properties with standard values.
        /// </summary>
        public DoubleLimitToBooleanConverter()
        {
            Limit = 0;
        }

        #region IValueConverter Members

        /// <summary>
        /// Implementation of the <see cref="System.Windows.Data.IValueConverter" /> member.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if value greater or equal to the limit then return TRUE, otherwise return FALSE
            if (value is double)
            {
                return (double)value >= this.Limit;
            }

            return value;
        }

        /// <summary>
        /// Implementation of the <see cref="System.Windows.Data.IValueConverter" /> member.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // if value is TRUE, then return the LIMIT, otherwise return ZERO
            if (value is bool)
            {
                return value.Equals(true) ? this.Limit : 0;
            }

            return value;
        }

        #endregion // IValueConverter Members

        #region MarkupExtension "overrides"

        /// <summary>
        /// Overrides the abstract "ProvideValue" method from the "MarkupExtension" class
        /// </summary>
        /// <param name="serviceProvider">A service provider helper that can provide services for the markup extension.</param>
        /// <returns>
        /// The object value to set on the property where the extension is applied.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new DoubleLimitToBooleanConverter { Limit = this.Limit };
        }

        #endregion
    }
}
