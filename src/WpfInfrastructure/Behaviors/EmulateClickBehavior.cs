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
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Surface.Presentation.Controls;

namespace VP.FF.PT.Common.WpfInfrastructure.Behaviors
{
    /// <summary>
    /// Emulates mouse click event, based on a surface/touch event. The SurfaceScrollViewer hides the click event and
    /// supports only the dragging in the scroll area.
    /// </summary>
    public class EmulateClickBehavior : Behavior<SurfaceScrollViewer>
    {
        DateTime _timeStamp;
        Point _position;
        readonly TimeSpan _epsilon = new TimeSpan(0, 0, 0, 0, 80);
        const double EpsilonX = 20.0;
        const double EpsilonY = 20.0;

        protected override void OnAttached()
        {
            base.OnAttached();

            //var touchDevice = Tablet.TabletDevices.Cast<TabletDevice>().FirstOrDefault(dev => dev.Type == TabletDeviceType.Touch);

            AssociatedObject.AddHandler(SurfaceScrollViewer.PreviewTouchDownEvent, new RoutedEventHandler(OnTouchDown));
            AssociatedObject.AddHandler(SurfaceScrollViewer.PreviewTouchUpEvent, new RoutedEventHandler(OnTouchUp));
        }

        protected override void OnDetaching()
        {
            AssociatedObject.RemoveHandler(SurfaceScrollViewer.PreviewTouchDownEvent, new RoutedEventHandler(OnTouchDown));
            AssociatedObject.RemoveHandler(SurfaceScrollViewer.PreviewTouchUpEvent, new RoutedEventHandler(OnTouchUp));
        }

        /// <summary>
        /// Check if the touch event was a while ago or the mouse position changed. In this case, we leave, as we assume it was a dragging move
        /// or a long-click for a context menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTouchUp(object sender, RoutedEventArgs e)
        {
            var newTimeStamp = DateTime.Now;
            System.Diagnostics.Debug.WriteLine("Stamp " + newTimeStamp);

            var time = (newTimeStamp - _timeStamp);
            System.Diagnostics.Debug.WriteLine("comp " + time.Milliseconds);
            if (time > _epsilon) return;

            var compare = Mouse.GetPosition(AssociatedObject);
            if (compare.X - _position.X > EpsilonX) return;
            if (compare.Y - _position.Y > EpsilonY) return;

            System.Diagnostics.Debug.WriteLine("rising");
            var downArg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = Mouse.MouseDownEvent
            };
            InputManager.Current.ProcessInput(downArg);
            var upArg = new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = Mouse.MouseUpEvent
            };
            InputManager.Current.ProcessInput(upArg);
        }

        void OnTouchDown(object sender, RoutedEventArgs e)
        {
            _timeStamp = DateTime.Now;
            _position = Mouse.GetPosition(AssociatedObject);
            System.Diagnostics.Debug.WriteLine("Stamp " + _timeStamp);
        }
    }
}
