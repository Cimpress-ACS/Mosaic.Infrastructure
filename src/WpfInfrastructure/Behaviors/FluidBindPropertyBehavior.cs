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

using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace VP.FF.PT.Common.WpfInfrastructure.Behaviors
{
    /// <summary>
    ///     Allows animating the changes from databound properties.
    /// </summary>
    public class FluidBindPropertyBehavior : Behavior<FrameworkElement>
    {
        /// <summary>Backing DP for the Duration property</summary>
        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register(
            "Duration",
            typeof(Duration),
            typeof(FluidBindPropertyBehavior),
            new PropertyMetadata(new Duration(TimeSpan.FromSeconds(.25))));

        /// <summary>Backing DP for the PropertyName property</summary>
        public static readonly DependencyProperty PropertyNameProperty = DependencyProperty.Register(
            "PropertyName", typeof(string), typeof(FluidBindPropertyBehavior), new PropertyMetadata(null));

        /// <summary>
        /// The listener.
        /// </summary>
        private readonly BindingListener listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="FluidBindPropertyBehavior"/> class. 
        ///     Constructor
        /// </summary>
        public FluidBindPropertyBehavior()
        {
            this.listener = new BindingListener(this.HandleBindingValueChanged);
        }

        /// <summary>
        ///     Binding to the value which is to be animated to.
        /// </summary>
        public Binding Binding
        {
            get
            {
                return this.listener.Binding;
            }

            set
            {
                this.listener.Binding = value;
            }
        }

        /// <summary>
        ///     Duration of the animation
        /// </summary>
        public Duration Duration
        {
            get
            {
                return (Duration)this.GetValue(DurationProperty);
            }

            set
            {
                this.SetValue(DurationProperty, value);
            }
        }

        /// <summary>
        ///     Name of the property to be set.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return (string)this.GetValue(PropertyNameProperty);
            }

            set
            {
                this.SetValue(PropertyNameProperty, value);
            }
        }

        /// <summary>
        ///     Perform initialization.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            this.listener.Element = this.AssociatedObject;
        }

        /// <summary>
        ///     Perform cleanup.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.listener.Element = null;
        }

        /// <summary>
        /// The handle binding value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HandleBindingValueChanged(object sender, BindingChangedEventArgs e)
        {
            object value = this.listener.Value;
            FrameworkElement target = this.AssociatedObject;
            AnimateTo(target, this.PropertyName, value, this.Duration, false);
        }

        /// <summary>
        /// The animate to.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="duration">
        /// The duration.
        /// </param>
        /// <param name="increment">
        /// The increment.
        /// </param>
        /// <exception cref="ArgumentException">
        /// </exception>
        internal static void AnimateTo(
            FrameworkElement target, string propertyName, object value, Duration duration, bool increment)
        {

            if (!string.IsNullOrEmpty(propertyName) && target != null)
            {
                PropertyInfo property = target.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    throw new ArgumentException(
                        "Cannot find property " + propertyName + " on object " + target.GetType().Name);
                }

                if (!property.CanWrite)
                {
                    throw new ArgumentException(
                        "Property is read-only " + propertyName + " on object " + target.GetType().Name);
                }

                object toValue = value;
                TypeConverter typeConverter = ConverterHelper.GetTypeConverter(property.PropertyType);
                Exception innerException = null;
                try
                {
                    if (((typeConverter != null) && (value != null)) && typeConverter.CanConvertFrom(value.GetType()))
                    {
                        toValue = typeConverter.ConvertFrom(value);
                    }

                    object fromValue = property.GetValue(target, null);

                    if (increment)
                    {
                        toValue = Add(toValue, fromValue);
                    }

                    if (duration.HasTimeSpan)
                    {
                        Timeline timeline = null;
                        if ((typeof(FrameworkElement).IsAssignableFrom(target.GetType())
                             && ((propertyName == "Width") || (propertyName == "Height")))
                            && double.IsNaN((double)fromValue))
                        {
                            fromValue = propertyName == "Width" ? target.ActualWidth : target.ActualHeight;
                        }

                        var storyboard = new Storyboard();
                        if (typeof(double).IsAssignableFrom(property.PropertyType))
                        {
                            var animation = new DoubleAnimation { From = (double)fromValue };
                            if (toValue != null)
                            {
                                var actualVal = Double.NaN;
                                Double.TryParse(toValue.ToString(), out actualVal);
                                animation.To = actualVal;
                            }
                            timeline = animation;
                        }
                        else if (typeof(Color).IsAssignableFrom(property.PropertyType))
                        {
                            var animation2 = new ColorAnimation { From = (Color)fromValue };
                            if (toValue != null)
                            {
                                animation2.To = (Color)toValue;
                            }
                            timeline = animation2;
                        }
                        else if (typeof(Point).IsAssignableFrom(property.PropertyType))
                        {
                            if (toValue != null)
                            {
                                var animation3 = new PointAnimation { From = (Point)fromValue, To = (Point)toValue };
                                timeline = animation3;
                            }
                        }
                        else
                        {
                            var frames = new ObjectAnimationUsingKeyFrames();
                            var frame = new DiscreteObjectKeyFrame
                            {
                                KeyTime = KeyTime.FromTimeSpan(new TimeSpan(0L)),
                                Value = fromValue
                            };
                            var frame2 = new DiscreteObjectKeyFrame
                            {
                                KeyTime = KeyTime.FromTimeSpan(duration.TimeSpan),
                                Value = toValue
                            };
                            frames.KeyFrames.Add(frame);
                            frames.KeyFrames.Add(frame2);
                            timeline = frames;
                        }

                        if (timeline != null)
                        {
                            timeline.Duration = duration;
                            storyboard.Children.Add(timeline);
                        }
                        Storyboard.SetTarget(storyboard, target);
                        Storyboard.SetTargetProperty(storyboard, new PropertyPath(property.Name, new object[0]));
                        storyboard.Begin();
                    }
                    else
                    {
                        property.SetValue(target, toValue, new object[0]);
                    }
                }
                catch (FormatException exception2)
                {
                    innerException = exception2;
                }
                catch (ArgumentException exception3)
                {
                    innerException = exception3;
                }
                catch (MethodAccessException exception4)
                {
                    innerException = exception4;
                }

                if (innerException != null)
                {
                    throw new ArgumentException(innerException.Message);
                }
            }
        }

        /// <summary>
        /// The add.
        /// </summary>
        /// <param name="a">
        /// The a.
        /// </param>
        /// <param name="b">
        /// The b.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// </exception>
        internal static object Add(object a, object b)
        {
            if (a.GetType() != b.GetType())
            {
                throw new Exception("Types must match");
            }

            Type type = a.GetType();
            if (type == typeof(double))
            {
                return (double)a + (double)b;
            }
            if (type == typeof(int))
            {
                return (int)a + (int)b;
            }
            if (type == typeof(string))
            {
                return (string)a + (string)b;
            }
            if (type == typeof(float))
            {
                return (float)a + (float)b;
            }

            MethodInfo add = type.GetMethod("op_Addition");
            if (add != null)
            {
                return add.Invoke(null, new[] { a, b });
            }

            throw new Exception("Unable to add type " + type);
        }

        /// <summary>
        /// Notification that the binding property has changed.
        /// </summary>
        /// <param name="e">
        /// </param>
        protected virtual void OnBindingChanged(DependencyPropertyChangedEventArgs e)
        {
            this.listener.Binding = this.Binding;
        }
    }
}
