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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace VP.FF.PT.Common.WpfInfrastructure.Behaviors
{
    public class LoadedBehaviors
    {
        #region Loaded

        // Using a DependencyProperty as the backing store for Loaded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadedProperty =
            DependencyProperty.RegisterAttached("Loaded",
                typeof(ICommand),
                typeof(LoadedBehaviors),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback((obj, e) => LoadedCommandChanged(obj, (ICommand)e.NewValue))));

        private static void LoadedCommandChanged(DependencyObject d, ICommand command)
        {
            FrameworkElement element = (FrameworkElement)d;

            element.Loaded += (obj, e) =>
            {
                RoutedCommand com = (RoutedCommand)command;
                com.Execute(e, (IInputElement)obj);
            };
        }

        public static ICommand GetLoaded(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LoadedProperty);
        }

        public static void SetLoaded(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadedProperty, value);
        }

        #endregion

        #region Unloaded

        // Using a DependencyProperty as the backing store for Unloaded.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnloadedProperty =
            DependencyProperty.RegisterAttached("Unloaded",
                typeof(ICommand),
                typeof(LoadedBehaviors),
                new FrameworkPropertyMetadata(null,
                    new PropertyChangedCallback((obj, e) => UnloadedCommandChanged(obj, (ICommand)e.NewValue))));

        private static void UnloadedCommandChanged(DependencyObject d, ICommand command)
        {
            FrameworkElement element = (FrameworkElement)d;

            element.Unloaded += (obj, e) =>
            {
                RoutedCommand com = (RoutedCommand)command;
                com.Execute(e, (IInputElement)obj);
            };
        }

        public static ICommand GetUnloaded(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(UnloadedProperty);
        }

        public static void SetUnloaded(DependencyObject obj, ICommand value)
        {
            obj.SetValue(UnloadedProperty, value);
        }

        #endregion


        
    }
}
