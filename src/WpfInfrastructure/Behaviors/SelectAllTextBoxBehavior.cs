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


namespace VP.FF.PT.Common.WpfInfrastructure.Behaviors
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interactivity;

    /// <summary>
    /// Behavior that selects all the text of a textbox when user enters with click.
    /// </summary>
    public class SelectAllTextBoxBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseDown += AssociatedObjectPreviewMouseDown;
            AssociatedObject.GotFocus += AssociatedObjectGotFocus;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseDown -= AssociatedObjectPreviewMouseDown;
            AssociatedObject.GotFocus -= AssociatedObjectGotFocus;
            base.OnDetaching();
        }

        private void AssociatedObjectGotFocus(object sender, RoutedEventArgs eventArgs)
        {

            AssociatedObject.SelectAll();
        }

        private void AssociatedObjectPreviewMouseDown(object sender, MouseButtonEventArgs eventArgs)
        {
            // If its a triple click, select all text for the user.
            if (eventArgs.ClickCount == 3)
            {
                AssociatedObject.SelectAll();
                return;
            }

            if (!AssociatedObject.IsKeyboardFocusWithin)
            {
                // If the text box is not yet focussed, give it the focus and
                // stop further processing of this click event.
                AssociatedObject.Focus();
                eventArgs.Handled = true;
            }
        }
    }
}
