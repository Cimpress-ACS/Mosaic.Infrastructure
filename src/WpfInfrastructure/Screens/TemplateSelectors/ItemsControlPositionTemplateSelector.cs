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
using VP.FF.PT.Common.WpfInfrastructure.Extensions;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.TemplateSelectors
{
    public class ItemsControlPositionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SingleElementTemplate { get; set; }
        public DataTemplate FirstElementTemplate { get; set; }
        public DataTemplate MiddleElementTemplate { get; set; }
        public DataTemplate LastElementTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var parentItemsControl = VisualTreeHelpers.FindAncestorOrSelf<ItemsControl>(container);

            if ((item != null) && (parentItemsControl != null))
            {
                var items = parentItemsControl.Items;
                if (items.IsEmpty) { return null; }

                // if only one element is present
                if (items.Count == 1)
                {
                    return SingleElementTemplate;
                }

                var index = items.IndexOf(item);
                // if first item
                if (index == 0)
                {
                    return FirstElementTemplate;
                }
                // if last item
                if (!items.IsEmpty && (index == items.Count - 1))
                {
                    return LastElementTemplate;
                }
                return MiddleElementTemplate;
            }

            return null;
        }
    }
}
