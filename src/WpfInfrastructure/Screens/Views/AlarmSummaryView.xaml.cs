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

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Views
{

    /// <summary>
    /// Interaction logic for AlarmSummaryView.xaml
    /// </summary>
    public partial class AlarmSummaryView
    {
        private const string MinimizedVisualState = "Minimized";
        private const string MaximizedVisualState = "Maximized";

        public AlarmSummaryView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            MouseDoubleClick += AlarmSummaryView_MouseDoubleClick;
        }

        private void AlarmSummaryView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SwitchAlarmSummaryViewState();
        }

        private void SwitchAlarmSummaryViewState()
        {
            if (IsMinimized)
                IsMinimized = false;
            else if (!IsMinimized && CanMinimize)
                IsMinimized = true;
        }

        public static readonly DependencyProperty IsMinimizedProperty = DependencyProperty.Register(
            "IsMinimized", typeof(bool), typeof(AlarmSummaryView), new PropertyMetadata(true, OnMinimizedChanged));

        public bool IsMinimized
        {
            get { return (bool)GetValue(IsMinimizedProperty); }
            set { SetValue(IsMinimizedProperty, value); }
        }

        private static void OnMinimizedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs changedEventArgs)
        {
            var minimize = (bool)changedEventArgs.NewValue;
            var alarmSummaryBox = dependencyObject as AlarmSummaryView;
            if (alarmSummaryBox != null)
            {
                if (minimize && (alarmSummaryBox.CanMinimize))
                {
                    VisualStateManager.GoToState(alarmSummaryBox, MinimizedVisualState, true);
                }
                else
                {
                    VisualStateManager.GoToState(alarmSummaryBox, MaximizedVisualState, true);
                }
            }
        }

        public static readonly DependencyProperty CanMinimizeProperty = DependencyProperty.Register(
            "CanMinimize", typeof(bool), typeof(AlarmSummaryView), new PropertyMetadata(true, OnCanMinimizeChanged));

        public bool CanMinimize
        {
            get { return (bool)GetValue(CanMinimizeProperty); }
            set { SetValue(CanMinimizeProperty, value); }
        }

        private static void OnCanMinimizeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs changedEventArgs)
        {
            var canMinimize = (bool)changedEventArgs.NewValue;
            var alarmSummaryBox = dependencyObject as AlarmSummaryView;
            if (alarmSummaryBox != null)
            {
                if (!canMinimize)
                {
                    dependencyObject.SetValue(IsMinimizedProperty, false);
                }
                else
                {
                    dependencyObject.SetValue(IsMinimizedProperty, true);
                }
            }
        }
    }
}
