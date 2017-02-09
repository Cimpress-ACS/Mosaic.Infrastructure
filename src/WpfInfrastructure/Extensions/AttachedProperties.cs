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


using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace VP.FF.PT.Common.WpfInfrastructure.Extensions
{
    public class AttachedProperties
    {
        #region AdditionalBackground Property

        public static readonly DependencyProperty AdditionalBackgroundProperty =
                DependencyProperty.RegisterAttached("AdditionalBackground", typeof(Brush), typeof(AttachedProperties), new PropertyMetadata(null));

        [AttachedPropertyBrowsableForType(typeof(Control))]
        public static void SetAdditionalBackground(UIElement element, Brush value)
        {
            element.SetValue(AdditionalBackgroundProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Control))]
        public static Brush GetAdditionalBackground(UIElement element)
        {
            return (Brush)element.GetValue(AdditionalBackgroundProperty);
        }

        #endregion

        #region Icon Property

        public static readonly DependencyProperty IconProperty =
                DependencyProperty.RegisterAttached("Icon", typeof(string), typeof(AttachedProperties), new PropertyMetadata(string.Empty));

        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetIcon(UIElement element, string value)
        {
            element.SetValue(IconProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static string GetIcon(UIElement element)
        {
            return (string)element.GetValue(IconProperty);
        }

        #endregion

        #region Text Property

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("AttachedText", typeof (string), typeof (AttachedProperties),
                new PropertyMetadata(string.Empty));

        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetAttachedText(UIElement element, string value)
        {
            element.SetValue(TextProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof (Button))]
        public static string GetAttachedText(UIElement element)
        {
            return (string) element.GetValue(TextProperty);
        }
        #endregion

        #region IconStyle Property

        public static readonly DependencyProperty IconStyleProperty =
                DependencyProperty.RegisterAttached("IconStyle", typeof(Style), typeof(AttachedProperties));

        [AttachedPropertyBrowsableForType(typeof(ContentControl))]
        public static void SetIconStyle(UIElement element, Style value)
        {
            element.SetValue(IconStyleProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(ContentControl))]
        public static Style GetIconStyle(UIElement element)
        {
            return (Style)element.GetValue(IconStyleProperty);
        }

        #endregion

        #region ScaleFactor Property

        public static readonly DependencyProperty ScaleFactorProperty =
                DependencyProperty.RegisterAttached("ScaleFactor", typeof(double), typeof(AttachedProperties), new PropertyMetadata(1.0));

        [AttachedPropertyBrowsableForType(typeof(UserControl))]
        public static void SetScaleFactor(UIElement element, double value)
        {
            element.SetValue(ScaleFactorProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(UserControl))]
        public static double GetScaleFactor(UIElement element)
        {
            return (double)element.GetValue(ScaleFactorProperty);
        }

        #endregion

        #region AttachedCommand Property

        public static readonly DependencyProperty AttachedCommandProperty =
            DependencyProperty.RegisterAttached("AttachedCommand", typeof(ICommand), typeof(AttachedProperties),
                                                new FrameworkPropertyMetadata(null,
                                                                              FrameworkPropertyMetadataOptions.Inherits,
                                                                              OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }


        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static void SetAttachedCommand(UIElement element, ICommand value)
        {
            element.SetValue(AttachedCommandProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static ICommand GetAttachedCommand(UIElement element)
        {
            return (ICommand)element.GetValue(AttachedCommandProperty);
        }

        #endregion

        #region ScrollBarBrush Property

        public static readonly DependencyProperty ScrollBarBrushProperty =
            DependencyProperty.RegisterAttached("ScrollBarBrush", typeof(SolidColorBrush), typeof(AttachedProperties), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White), FrameworkPropertyMetadataOptions.Inherits));

        [AttachedPropertyBrowsableForType(typeof(ScrollBar))]
        public static void SetScrollBarBrush(UIElement element, SolidColorBrush value)
        {
            element.SetValue(ScrollBarBrushProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(ScrollBar))]
        public static SolidColorBrush GetScrollBarBrush(UIElement element)
        {
            return (SolidColorBrush)element.GetValue(ScrollBarBrushProperty);
        }

        #endregion

        #region CornerRadius Property

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register(
            "CornerRadius", typeof (CornerRadius), typeof (AttachedProperties));

        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static void SetCornerRadius(UIElement element, CornerRadius value)
        {
            element.SetValue(CornerRadiusProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(UIElement))]
        public static CornerRadius GetCornerRadius(UIElement element)
        {
            return (CornerRadius)element.GetValue(CornerRadiusProperty);
        }     

        #endregion
    }
}
