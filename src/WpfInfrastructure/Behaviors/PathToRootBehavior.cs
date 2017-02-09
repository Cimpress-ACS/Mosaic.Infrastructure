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


using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace VP.FF.PT.Common.WpfInfrastructure.Behaviors
{
    /// <summary>
    /// This class provides an AttachedProperty used by the PathToRootBehavior.
    /// </summary>
    public class PathToRoot
    {
        /// <summary>
        /// Property to flag an TreeViewItem, that is located between the root element and the selected element. 
        /// </summary>
        public static readonly DependencyProperty OnPathToRootProperty =
            DependencyProperty.RegisterAttached("OnPathToRoot", typeof(bool), typeof(PathToRoot), new PropertyMetadata(false));

        public static void SetOnPathToRoot(UIElement element, bool value)
        {
            element.SetValue(OnPathToRootProperty, value);
        }

        public static bool GetOnPathToRoot(UIElement element)
        {
            return (bool)element.GetValue(OnPathToRootProperty);
        }
    }

    /// <summary>
    /// The PathToRootBehavior flags all TreeViewItems located between the root item and the currently selected item.
    /// </summary>
    public class PathToRootBehavior : Behavior<TreeView>
    {
        private readonly List<ItemsControl> _itemsToRoot = new List<ItemsControl>();

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(OnTreeViewItemExpanded));
        }

        protected override void OnDetaching()
        {
            AssociatedObject.RemoveHandler(TreeViewItem.SelectedEvent, new RoutedEventHandler(OnTreeViewItemExpanded));
        }

        private void OnTreeViewItemExpanded(object sender, RoutedEventArgs e)
        {
            foreach (var oldItem in _itemsToRoot)
            {
                PathToRoot.SetOnPathToRoot(oldItem, false);
            }
            
            _itemsToRoot.Clear();

            var item = e.OriginalSource as TreeViewItem;
            if (item == null)
            {
                return;
            }
            
            ItemsControl parent = item;
            do
            {
                parent = ItemsControl.ItemsControlFromItemContainer(parent);
                if (parent != null)
                {
                    PathToRoot.SetOnPathToRoot(parent, true);
                    _itemsToRoot.Add(parent);
                }
            } while (parent != null);
        }
    }
}
