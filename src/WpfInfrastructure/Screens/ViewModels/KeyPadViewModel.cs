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
using System.Runtime.InteropServices;
using System.Windows.Input;
using Action = System.Action;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using Screen = Caliburn.Micro.Screen;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    public class KeyPadViewModel : Screen
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        public const int KeyeventfKeydown = 0x0001; //Key down flag
        public const int KeyeventfKeyup = 0x0002; //Key up flag
        public const int Enter = 0x0D; // enter keycode.
        public const int Esc = 0x1B; // 
        private string _text;

        public KeyPadViewModel()
        {
            Text = "";

        }

        public String Text
        {
            get { return _text; }
            set
            {
                _text = value;
                NotifyOfPropertyChange(() => Text);
            }
        }

        /// <summary>
        /// Paste this function wherever you need it(ViewModel) and change Text to the respective property(binded to the TextBox)
        /// Apply Style="{StaticResource KeyPadTextBoxStyle}" to the TextBox you want to have KeyPad on (View).
        /// Add the following lines to the View(resource):
        /*<UserControl.Resources>
            <ResourceDictionary>
                <ResourceDictionary.MergedDictionaries>
                    <ResourceDictionary 
                        Source="pack://application:,,,/VP.FF.PT.Common.WpfInfrastructure;component/Styles/Resources/Keypicker.xaml" />
                </ResourceDictionary.MergedDictionaries>
            </ResourceDictionary>
          </UserControl.Resources>*/
        /// </summary>
        /// <param name="value">keypad key</param>
        public void OnClick(string value)
        {
            switch (value)
            {
                case ".":
                    if (Text.Contains("."))
                        break;
                    Text += value;
                    break;
                case "Enter":
                    keybd_event(Enter, 0, KeyeventfKeydown, 0);
                    keybd_event(Enter, 0, KeyeventfKeyup, 0);
                    break;
                case "Esc":
                    keybd_event(Esc, 0, KeyeventfKeydown, 0);
                    keybd_event(Esc, 0, KeyeventfKeyup, 0);
                    break;
                case "0":
                    Text += value;
                    break;
                case "1":
                    SendKeys.Send(Key.NumPad0);
                    Text += value;
                    break;
                case "2":
                    Text += value;
                    break;
                case "3":
                    Text += value;
                    break;
                case "4":
                    Text += value;
                    break;
                case "5":
                    Text += value;
                    break;
                case "6":
                    Text += value;
                    break;
                case "7":
                    Text += value;
                    break;
                case "8":
                    Text += value;
                    break;
                case "9":
                    Text += value;
                    break;
                default:
                    if (Text.Length < 2)
                    {
                        Text = "";
                        break;
                    }
                    Text = Text.Remove(Text.Length - 1);
                    break;
            }

        }

    }

    public static class SendKeys
    {
        /// <summary>
        ///   Sends the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public static void Send(Key key)
        {

            if (Keyboard.PrimaryDevice != null)
            {
                if (Keyboard.PrimaryDevice.ActiveSource != null)
                {
                    var e1 = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key) { RoutedEvent = Keyboard.KeyDownEvent };
                    bool pp = InputManager.Current.ProcessInput(e1);
                }
            }
        }
    }
}
