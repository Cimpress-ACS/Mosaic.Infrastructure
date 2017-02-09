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
using System.Windows.Media;

namespace VP.FF.PT.Common.WpfInfrastructure.Controls
{
    public class BreadcrumbBarButton
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for GeometryLeft.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GeometryLeftProperty =
                    DependencyProperty.RegisterAttached("GeometryLeft", typeof(StreamGeometry), typeof(BreadcrumbBarButton));

        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetGeometryLeft(UIElement element, StreamGeometry value)
        {
            element.SetValue(GeometryLeftProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static StreamGeometry GetGeometryLeft(UIElement element)
        {
            return (StreamGeometry)element.GetValue(GeometryLeftProperty);
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GeometryRight.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GeometryRightProperty =
                    DependencyProperty.RegisterAttached("GeometryRight", typeof(StreamGeometry), typeof(BreadcrumbBarButton));

        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static void SetGeometryRight(UIElement element, StreamGeometry value)
        {
            element.SetValue(GeometryRightProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Button))]
        public static StreamGeometry GetGeometryRight(UIElement element)
        {
            return (StreamGeometry)element.GetValue(GeometryRightProperty);
        }
    }
}
