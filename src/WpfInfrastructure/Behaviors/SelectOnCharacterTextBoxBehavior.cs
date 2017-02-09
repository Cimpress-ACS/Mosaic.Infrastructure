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


using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace VP.FF.PT.Common.WpfInfrastructure.Behaviors
{
    public class SelectOnCharacterTextBoxBehavior : Behavior<TextBox>
    {
        // ignore next SelectionChanged event
        private bool _changed;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObjectSelectionChanged;
            AssociatedObject.PreviewKeyDown += AssociatedObjectPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObjectSelectionChanged;
            AssociatedObject.PreviewKeyDown -= AssociatedObjectPreviewKeyDown;
        }

        private void AssociatedObjectSelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_changed)
            {
                _changed = false;
                return;
            }

            var caretIndex = AssociatedObject.CaretIndex;
            var length = AssociatedObject.Text.Length;

            if (caretIndex >= length)
                caretIndex = length - 1;

            if (caretIndex < 0)
                return;

            _changed = true;
            AssociatedObject.Select(caretIndex, 1);
        }

        private void AssociatedObjectPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                var caretIndex = AssociatedObject.CaretIndex;

                if (caretIndex <= 0)
                    return;

                _changed = true;
                AssociatedObject.Select(caretIndex - 1, 1);
            }
        }
    }
}
