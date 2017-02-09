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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VP.FF.PT.Common.WpfInfrastructure.CustomControls
{
    /// <summary>
    /// Interaction logic for CircularProgressBar.xaml
    /// </summary>
    public partial class CircularProgressBar : IDisposable
    {
        private DispatcherTimer _animationTimer;

        public CircularProgressBar()
        {
            InitializeComponent();

            _animationTimer = new DispatcherTimer(
                DispatcherPriority.ContextIdle, Dispatcher);
            _animationTimer.Interval = TimeSpan.FromMilliseconds(100);
            _animationTimer.Tick += HandleAnimationTick;
        }

        private void Start()
        {
            _animationTimer.Start();
        }

        private void Stop()
        {
            _animationTimer.Stop();
        }

        private void HandleAnimationTick(object sender, EventArgs e)
        {
            //36 represents the angle  with which the image is rotated at one tick. a steeper angle means a smoother animation(more FPS) while a lower angle means a sloppier animation(fewer FPS)
            //with this number it makes the full circle in 10 ticks(=1s)
            SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI;
            const double step = Math.PI * 2 / 10.0;

            SetPosition(C0, offset, 0.0, step);
            SetPosition(C1, offset, 1.0, step);
            SetPosition(C2, offset, 2.0, step);
            SetPosition(C3, offset, 3.0, step);
            SetPosition(C4, offset, 4.0, step);
            SetPosition(C5, offset, 5.0, step);
            SetPosition(C6, offset, 6.0, step);
            SetPosition(C7, offset, 7.0, step);
            SetPosition(C8, offset, 8.0, step);
        }

        private void SetPosition(Ellipse ellipse, double offset,
            double posOffSet, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, 50.0
                + Math.Sin(offset + posOffSet * step) * 50.0);

            ellipse.SetValue(Canvas.TopProperty, 50
                + Math.Cos(offset + posOffSet * step) * 50.0);
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void HandleVisibleChanged(object sender,
            DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool) e.NewValue;

            if (isVisible)
                Start();
            else
                Stop();
        }

        public void Dispose()
        {
            _animationTimer.Tick -= HandleAnimationTick;

            if (_animationTimer != null)
                _animationTimer = null;
        }
    }
}
