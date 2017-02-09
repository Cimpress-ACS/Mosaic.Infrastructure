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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace VP.FF.PT.Common.WpfInfrastructure.Behaviors
{
    /// <summary>
    /// InputMask for Textbox with 2 Properties: <see cref="InputMask"/>, <see cref="PromptChar"/>.
    /// </summary>
    /// <remarks>
    /// http://blindmeis.wordpress.com/2010/06/01/wpf-masked-textbox-behavior/
    /// </remarks>
    public class TextBoxInputMaskBehavior : Behavior<TextBox>
    {
        #region DependencyProperties

        public static readonly DependencyProperty InputMaskProperty =
            DependencyProperty.Register("InputMask", typeof (string), typeof (TextBoxInputMaskBehavior), null);

        public static readonly DependencyProperty PromptCharProperty =
            DependencyProperty.Register("PromptChar", typeof (char), typeof (TextBoxInputMaskBehavior),
                                        new PropertyMetadata('_'));

        public string InputMask
        {
            get { return (string) GetValue(InputMaskProperty); }
            set { SetValue(InputMaskProperty, value); }
        }

        public char PromptChar
        {
            get { return (char) GetValue(PromptCharProperty); }
            set { SetValue(PromptCharProperty, value); }
        }

        #endregion

        public MaskedTextProvider Provider { get; private set; }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObjectLoaded;
            AssociatedObject.PreviewTextInput += AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown += AssociatedObjectPreviewKeyDown;

            DataObject.AddPastingHandler(AssociatedObject, Pasting);
        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= AssociatedObjectLoaded;
            AssociatedObject.PreviewTextInput -= AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown -= AssociatedObjectPreviewKeyDown;

            DataObject.RemovePastingHandler(AssociatedObject, Pasting);
        }

        /*
            Mask Character Accepts Required?  
            0  Digit (0-9)  Required  
            9  Digit (0-9) or space  Optional  
            #  Digit (0-9) or space  Required  
            L  Letter (a-z, A-Z)  Required  
            ?  Letter (a-z, A-Z)  Optional  
            &  Any character  Required  
            C  Any character  Optional  
            A  Alphanumeric (0-9, a-z, A-Z)  Required  
            a  Alphanumeric (0-9, a-z, A-Z)  Optional  
               Space separator  Required 
            .  Decimal separator  Required  
            ,  Group (thousands) separator  Required  
            :  Time separator  Required  
            /  Date separator  Required  
            $  Currency symbol  Required  

            In addition, the following characters have special meaning:

            Mask Character  Meaning  
            <  All subsequent characters are converted to lower case  
            >  All subsequent characters are converted to upper case  
            |  Terminates a previous < or >  
            \  Escape: treat the next character in the mask as literal text rather than a mask symbol  
        */

        private void AssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            Provider = new MaskedTextProvider(InputMask, CultureInfo.CurrentCulture);
            Provider.Set(AssociatedObject.Text);
            Provider.PromptChar = PromptChar;
            AssociatedObject.Text = Provider.ToDisplayString();

            //seems the only way that the text is formatted correct, when source is updated
            DependencyPropertyDescriptor textProp = DependencyPropertyDescriptor.FromProperty(TextBox.TextProperty,
                                                                                              typeof (TextBox));
            if (textProp != null)
            {
                textProp.AddValueChanged(AssociatedObject, (s, args) => UpdateText());
            }
        }

        private void AssociatedObjectPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TreatSelectedText();

            int position = GetNextCharacterPosition(AssociatedObject.SelectionStart);

            if (Keyboard.IsKeyToggled(Key.Insert))
            {
                if (Provider.Replace(e.Text, position))
                    position++;
            }
            else
            {
                if (Provider.InsertAt(e.Text, position))
                    position++;
            }

            position = GetNextCharacterPosition(position);

            RefreshText(position);

            e.Handled = true;
        }

        private void AssociatedObjectPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) //handle the space
            {
                TreatSelectedText();

                int position = GetNextCharacterPosition(AssociatedObject.SelectionStart);

                if (Provider.InsertAt(" ", position))
                    RefreshText(position);

                e.Handled = true;
            }

            if (e.Key == Key.Back) //handle the back space
            {
                if (TreatSelectedText())
                {
                    RefreshText(AssociatedObject.SelectionStart);
                }
                else
                {
                    if (AssociatedObject.SelectionStart != 0)
                    {
                        if (Provider.RemoveAt(AssociatedObject.SelectionStart - 1))
                            RefreshText(AssociatedObject.SelectionStart - 1);
                    }
                }

                e.Handled = true;
            }

            if (e.Key == Key.Delete) //handle the delete key
            {
                //treat selected text
                if (TreatSelectedText())
                {
                    RefreshText(AssociatedObject.SelectionStart);
                }
                else
                {
                    if (Provider.RemoveAt(AssociatedObject.SelectionStart))
                        RefreshText(AssociatedObject.SelectionStart);
                }

                e.Handled = true;
            }
        }

        private void Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof (string)))
            {
                var pastedText = (string) e.DataObject.GetData(typeof (string));

                TreatSelectedText();

                int position = GetNextCharacterPosition(AssociatedObject.SelectionStart);

                if (Provider.InsertAt(pastedText, position))
                {
                    RefreshText(position);
                }
            }

            e.CancelCommand();
        }

        private void UpdateText()
        {
            //check Provider.Text + TextBox.Text
            if (Provider.ToDisplayString().Equals(AssociatedObject.Text))
                return;

            //use provider to format
            bool success = Provider.Set(AssociatedObject.Text);

            //ui and mvvm/codebehind should be in sync
            SetText(success ? Provider.ToDisplayString() : AssociatedObject.Text);
        }

        private bool TreatSelectedText()
        {
            if (AssociatedObject.SelectionLength > 0)
            {
                return Provider.RemoveAt(AssociatedObject.SelectionStart,
                                         AssociatedObject.SelectionStart + AssociatedObject.SelectionLength - 1);
            }
            return false;
        }

        private void RefreshText(int position)
        {
            SetText(Provider.ToDisplayString());
            AssociatedObject.SelectionStart = position;
        }

        private void SetText(string text)
        {
            AssociatedObject.Text = String.IsNullOrWhiteSpace(text) ? String.Empty : text;
        }

        private int GetNextCharacterPosition(int startPosition)
        {
            int position = Provider.FindEditPositionFrom(startPosition, true);

            if (position == -1)
                return startPosition;
            
            return position;
        }
    }
}
