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
using System.Windows.Media;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Controls
{
    [TemplateVisualState(Name = "Empty", GroupName = "VisualStateGroup")]
    [TemplateVisualState(Name = "Pending", GroupName = "VisualStateGroup")]
    [TemplateVisualState(Name = "Loaded", GroupName = "VisualStateGroup")]
    [TemplateVisualState(Name = "Flagged", GroupName = "VisualStateGroup")]

    [TemplateVisualState(Name = "Back", GroupName = "VisualStateGroup")]
    [TemplateVisualState(Name = "Back_Flagged", GroupName = "VisualStateGroup")]

    //[TemplateVisualState(Name = "Front", GroupName = "DirectionVisualStateGroup")]
    //[TemplateVisualState(Name = "Back", GroupName = "DirectionVisualStateGroup")]


    public class ShirtControl : ContentControl
    {

        public string ShirtType
        {
            get { return (string)GetValue(ShirtTypeProperty); }
            set { SetValue(ShirtTypeProperty, value); }
        }

        public static readonly DependencyProperty ShirtTypeProperty = DependencyProperty.Register("ShirtType", typeof(string), typeof(ShirtControl), new PropertyMetadata("", ShirtTypeChanged));

        private static void ShirtTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Code for dealing with your property changes
        }

        public bool IsBack
        {
            get { return (bool)GetValue(IsBackProperty); }
            set { SetValue(IsBackProperty, value); }
        }

        public static readonly DependencyProperty IsBackProperty = DependencyProperty.Register("IsBack", typeof(bool), typeof(ShirtControl), new PropertyMetadata(IsBackChanged));

        private static void IsBackChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;

            var shirtControl = (ShirtControl)d;
            shirtControl.goToState(shirtControl.ShirtState.ToString());     
        }


        public bool IsHeadVisible
        {
            get { return (bool)GetValue(IsHeadVisibleProperty); }
            set { SetValue(IsHeadVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsHeadVisibleProperty = DependencyProperty.Register("IsHeadVisible", typeof(bool), typeof(ShirtControl), new PropertyMetadata(false, IsHeadVisibleChanged));

        private static void IsHeadVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Code for dealing with your property changes
        }

        public bool IsClampVisible
        {
            get { return (bool)GetValue(IsClampVisibleProperty); }
            set { SetValue(IsClampVisibleProperty, value); }
        }

        public static readonly DependencyProperty IsClampVisibleProperty = DependencyProperty.Register("IsClampVisible", typeof(bool), typeof(ShirtControl), new PropertyMetadata(true, IsClampVisibleChanged));

        private static void IsClampVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Code for dealing with your property changes
        }


        public ShirtState ShirtState
        {
            get { return (ShirtState)GetValue(ShirtStateProperty); }
            set { SetValue(ShirtStateProperty, value); }
        }
        public static readonly DependencyProperty ShirtStateProperty = DependencyProperty.Register("ShirtState", typeof(ShirtState), typeof(ShirtControl), new PropertyMetadata(ShirtStateChanged));

        private static void ShirtStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            var shirtControl = (ShirtControl) d;
            shirtControl.goToState(e.NewValue.ToString());
        }


        public Brush Color
        {
            get { return (Brush)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register("Color", typeof(Brush), typeof(ShirtControl), new PropertyMetadata(ColorChanged));

        private static void ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Code for dealing with your property changes
        }


        public double Size
        {
            get { return (double)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register("Size", typeof(double), typeof(ShirtControl), new PropertyMetadata(OnSizeChanged));

        private static void OnSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Code for dealing with your property changes
        }



        public OriginLabel OriginLabel
        {
            get { return (OriginLabel)GetValue(OriginLabelProperty); }
            set { SetValue(OriginLabelProperty, value); }
        }

        public static readonly DependencyProperty OriginLabelProperty = DependencyProperty.Register("OriginLabel", typeof(OriginLabel), typeof(ShirtControl), new PropertyMetadata(OriginLabelChanged));

        private static void OriginLabelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {          
            // Code for dealing with your property changes
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.goToState(ShirtState.ToString());
        }

        private void goToState(String state)
        {
            if (IsBack)
            {
                if (state.Equals("Loaded"))
                {
                    VisualStateManager.GoToState(this, "Back", true);
                }
                else if (state.Equals("Flagged"))
                {
                    VisualStateManager.GoToState(this, "Back_Flagged", true);
                }
                return;
            }
            VisualStateManager.GoToState(this, state, true);
        }
    }
}
