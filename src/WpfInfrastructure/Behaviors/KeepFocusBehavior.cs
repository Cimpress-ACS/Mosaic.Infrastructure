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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace VP.FF.PT.Common.WpfInfrastructure.Behaviors
{
    /// <summary>
    /// Keeps the focus inside a TextBox.
    /// </summary>
    public class KeepFocusBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            AssociatedObject.LostKeyboardFocus += OnLostKeyboardFocus;
            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.TextChanged += OnTextChanged;

            this.AssociatedObject.Unloaded += delegate
            {
                Interaction.GetBehaviors(this.AssociatedObject).Remove(this);
            };

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.LostKeyboardFocus -= OnLostKeyboardFocus;
            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.TextChanged -= OnTextChanged;

            base.OnDetaching();
        }

        private void OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            AssociatedObject.Focus();
            Keyboard.Focus(AssociatedObject);
        }

        private void OnLoaded(object sender, EventArgs e)
        {
            AssociatedObject.Focus();
            Keyboard.Focus(AssociatedObject);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(AssociatedObject.Text))
            {
                AssociatedObject.Focus();
                Keyboard.Focus(AssociatedObject);
            }
        }
    }
}
