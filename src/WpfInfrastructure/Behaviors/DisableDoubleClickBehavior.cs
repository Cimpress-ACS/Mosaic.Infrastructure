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
using System.Windows.Interactivity;

namespace VP.FF.PT.Common.WpfInfrastructure.Behaviors
{
    /// <summary>
    /// The DisableDoubleClickBehavior disables the double click event on a TreeViewItem.
    /// </summary>
    public class DisableDoubleClickBehavior : Behavior<TreeView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(TreeViewItem.PreviewMouseDoubleClickEvent, new RoutedEventHandler(OnTreeViewItemDoubleClicked));
        }

        protected override void OnDetaching()
        {
            AssociatedObject.RemoveHandler(TreeViewItem.PreviewMouseDoubleClickEvent, new RoutedEventHandler(OnTreeViewItemDoubleClicked));
        }

        private void OnTreeViewItemDoubleClicked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
        }
    }
}
