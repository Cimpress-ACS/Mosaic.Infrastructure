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
using System.Windows.Input;
using System.Windows.Interactivity;

namespace VP.FF.PT.Common.WpfInfrastructure.Extensions
{
    public class DragGestureTrigger : TriggerBase<FrameworkElement>
    {
        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        };

        private Point _startPoint;
        private bool _isCleanedUp;
        private bool _isAlreadySwiped;

        public Direction DragDirection
        {
            get { return (Direction)GetValue(DragDirectionProperty); }
            set { SetValue(DragDirectionProperty, value); }
        }

        public static readonly DependencyProperty DragDirectionProperty = DependencyProperty.Register(
            "DragDirection",
            typeof(Direction),
            typeof(DragGestureTrigger),
            new PropertyMetadata(Direction.Up));

        public double DragThreshold
        {
            get { return (double)GetValue(DragThresholdProperty); }
            set { SetValue(DragThresholdProperty, value); }
        }

        public static readonly DependencyProperty DragThresholdProperty = DependencyProperty.Register(
            "DragThreshold",
            typeof(double),
            typeof(DragGestureTrigger),
            new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.Unloaded += OnUnloaded;
        }

        protected override void OnDetaching()
        {
            Cleanup();
            base.OnDetaching();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.PreviewTouchDown += OnTouchDown;
            AssociatedObject.PreviewTouchMove += OnTouchMove;
            AssociatedObject.PreviewTouchUp += OnTouchUp;
            AssociatedObject.PreviewMouseDown += OnMouseDown;
            AssociatedObject.PreviewMouseMove += OnMouseMove;
            AssociatedObject.PreviewMouseUp += OnMouseUp;

            if (DragThreshold.Equals(0.0))
            {
                DragThreshold = double.Parse((AssociatedObject.ActualHeight / 2).ToString());
            }

            _isAlreadySwiped = false;
        }

        /// <summary>
        /// Ruft die Cleanup Methode auf, da die OnDetaching Methode nicht immer zuverlässig aufgerufen wird. Wenn dadurch Events nicht deregistriert werden, könnten Memory Leaks entstehen.
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // TODO: If Behavior is used on Tab, Unloaded is called, when the tab is changed. Check if Cleanup can be used, when no tabs are used.
            //Cleanup();
        }

        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            _isAlreadySwiped = false;
            _startPoint = e.GetTouchPoint(AssociatedObject).Position;

            // when a touch was recognized we unsubscribe mouse-events to prevent double calls (touch-events call mouse-events anyway)
            AssociatedObject.PreviewMouseDown -= OnMouseDown;
            AssociatedObject.PreviewMouseMove -= OnMouseMove;
            AssociatedObject.PreviewMouseUp -= OnMouseUp;
        }

        private void OnTouchMove(object sender, TouchEventArgs e)
        {
            var currentTouchPosition = e.GetTouchPoint(AssociatedObject).Position;

            if (!_isAlreadySwiped && MovementOverThreshold(_startPoint, currentTouchPosition, DragThreshold))
            {
                RecognizeDragGesture(currentTouchPosition);
                if (_isAlreadySwiped)
                {
                    e.Handled = true;
                }
            }
        }

        private bool MovementOverThreshold(Point start, Point end, double threshold)
        {
            if (end.Y > (start.Y + threshold) || end.Y < (start.Y - threshold) || end.X > (start.X + threshold) || end.X < (start.X - threshold))
            {
                return true;
            }

            return false;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            _isAlreadySwiped = false;
            //Mouse.Capture(AssociatedObject);
            _startPoint = e.GetPosition(AssociatedObject);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var currentPosition = e.GetPosition(AssociatedObject);
            if (e.LeftButton == MouseButtonState.Pressed && !_isAlreadySwiped && MovementOverThreshold(_startPoint, currentPosition, DragThreshold))
            {
                RecognizeDragGesture(currentPosition);
                if (_isAlreadySwiped)
                {
                    e.Handled = true;
                }
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isAlreadySwiped)
            {
                //Mouse.Capture(null);
                e.Handled = true;
                _isAlreadySwiped = false;
            }
        }

        private void OnTouchUp(object sender, TouchEventArgs e)
        {
            // register mouse events again in case somebody uses mouse device now instead of touch screen
            AssociatedObject.PreviewMouseDown += OnMouseDown;
            AssociatedObject.PreviewMouseMove += OnMouseMove;
            AssociatedObject.PreviewMouseUp += OnMouseUp;

            if (_isAlreadySwiped)
            {
                e.Handled = true;
                _isAlreadySwiped = false;
            }
        }

        private void RecognizeDragGesture(Point currentTouchPosition)
        {
            // Swipe Down
            if (currentTouchPosition.Y > (_startPoint.Y + DragThreshold) && DragDirection == Direction.Down)
            {
                InvokeActions("down");
                _isAlreadySwiped = true;
            }

            // Swipe Up
            if (currentTouchPosition.Y < (_startPoint.Y - DragThreshold) && DragDirection == Direction.Up)
            {
                InvokeActions("up");
                _isAlreadySwiped = true;
            }

            // Swipe Right
            if (currentTouchPosition.X > (_startPoint.X + DragThreshold) && DragDirection == Direction.Right)
            {
                InvokeActions("right");
                _isAlreadySwiped = true;
            }

            // Swipe Left
            if (currentTouchPosition.X < (_startPoint.X - DragThreshold) && DragDirection == Direction.Left)
            {
                InvokeActions("left");
                _isAlreadySwiped = true;
            }
        }

        /// <summary>
        /// unsubscribe events to prevent a memory leak
        /// </summary>
        private void Cleanup()
        {
            if (!_isCleanedUp)
            {
                _isCleanedUp = true;
                AssociatedObject.Loaded -= OnLoaded;
                AssociatedObject.Unloaded -= OnUnloaded;
                AssociatedObject.PreviewTouchDown -= OnTouchDown;
                AssociatedObject.PreviewTouchMove -= OnTouchMove;
                AssociatedObject.PreviewTouchUp -= OnTouchUp;
                AssociatedObject.PreviewMouseDown -= OnMouseDown;
                AssociatedObject.PreviewMouseMove -= OnMouseMove;
                AssociatedObject.PreviewMouseUp -= OnMouseUp;
            }
        }
    }
}
