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


// --------------------------------------------------------------------------------------------------------------------
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.
// --------------------------------------------------------------------------------------------------------------------

namespace VP.FF.PT.Common.WpfInfrastructure.Behaviors
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Windows.Controls;
    using System.Windows.Markup;

    /// <summary>
    ///     Helper for converting objects to different types.
    /// </summary>
    public static class ConverterHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Attempts to convert the provided value to the specified type
        /// </summary>
        /// <param name="value">
        /// object to be converted
        /// </param>
        /// <param name="type">
        /// Type to be converted to
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public static object ConvertToType(object value, Type type)
        {
            if (value == null)
            {
                return null;
            }

            if (type.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            TypeConverter converter = GetTypeConverter(type);

            if (converter != null && converter.CanConvertFrom(value.GetType()))
            {
                value = converter.ConvertFrom(value);
                return value;
            }

            return null;
        }

        /// <summary>
        /// Finds the type converter for the specified type.
        /// </summary>
        /// <param name="type">
        /// </param>
        /// <returns>
        /// The <see cref="TypeConverter"/>.
        /// </returns>
        public static TypeConverter GetTypeConverter(Type type)
        {
            var attribute =
                (TypeConverterAttribute)Attribute.GetCustomAttribute(type, typeof(TypeConverterAttribute), false);
            if (attribute != null)
            {
                try
                {
                    Type converterType = Type.GetType(attribute.ConverterTypeName, false);
                    if (converterType != null)
                    {
                        return Activator.CreateInstance(converterType) as TypeConverter;
                    }
                }
                catch (Exception)
                {
                }
            }

            return new ConvertFromStringConverter(type);
        }

        #endregion
    }

    /// <summary>
    ///     General string to object converter which uses the internal
    ///     platforms type converters.
    /// </summary>
    public class ConvertFromStringConverter : TypeConverter
    {
        #region Fields

        /// <summary>
        /// The type.
        /// </summary>
        private readonly Type type;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertFromStringConverter"/> class. 
        /// General purpose converter that converts from a string to the specified type.
        /// </summary>
        /// <param name="type">
        /// </param>
        public ConvertFromStringConverter(Type type)
        {
            this.type = type;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Allow conversion from strings.
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="sourceType">
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return (sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Convert the value
        /// </summary>
        /// <param name="context">
        /// </param>
        /// <param name="culture">
        /// </param>
        /// <param name="value">
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
                if (this.type == typeof(bool))
                {
                    return bool.Parse(stringValue);
                }

                if (this.type.IsEnum)
                {
                    return Enum.Parse(this.type, stringValue, false);
                }

                var stringBuilder = new StringBuilder();
                stringBuilder.Append(
                    "<ContentControl xmlns='http://schemas.microsoft.com/client/2007' xmlns:c='"
                    + ("clr-namespace:" + this.type.Namespace + ";assembly="
                       + this.type.Assembly.FullName.Split(new[] { ',' })[0]) + "'>\n");
                stringBuilder.Append("<c:" + this.type.Name + ">\n");
                stringBuilder.Append(stringValue);
                stringBuilder.Append("</c:" + this.type.Name + ">\n");
                stringBuilder.Append("</ContentControl>");
#if SILVERLIGHT
				ContentControl instance = XamlReader.Load(stringBuilder.ToString()) as ContentControl;
#else
                var memoryStream = new MemoryStream();
                var streamWriter = new StreamWriter(memoryStream);
                streamWriter.Write(stringBuilder.ToString());
                memoryStream.Seek(0, SeekOrigin.Begin);
                var instance = XamlReader.Load(memoryStream) as ContentControl;
#endif
                if (instance != null)
                {
                    return instance.Content;
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

        #endregion
    }
}
