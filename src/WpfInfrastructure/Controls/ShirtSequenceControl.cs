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
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Controls
{
    [TemplateVisualState(Name = "Next", GroupName = "VisualStateGroup")]
    [TemplateVisualState(Name = "Jump", GroupName = "VisualStateGroup")]
    [TemplatePart(Name = "PART_LayoutRoot", Type = typeof(FrameworkElement))]
    public class ShirtSequenceControl : Control
    {
        public Shirt CurrentShirt
        {
            get { return (Shirt)GetValue(CurrentShirtProperty); }
            set { SetValue(CurrentShirtProperty, value); }
        }

        public static readonly DependencyProperty CurrentShirtProperty = DependencyProperty.Register("CurrentShirt", typeof(Shirt), typeof(ShirtSequenceControl), new PropertyMetadata(CurrentShirtChanged));

        private static void CurrentShirtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Code for dealing with your property changes
        }


        public Shirt RecentShirt
        {
            get { return (Shirt)GetValue(RecentShirtProperty); }
            set { SetValue(RecentShirtProperty, value); }
        }

        public static readonly DependencyProperty RecentShirtProperty = DependencyProperty.Register("RecentShirt", typeof(Shirt), typeof(ShirtSequenceControl), new PropertyMetadata(RecentShirtChanged));

        private static void RecentShirtChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Code for dealing with your property changes
        }

        public bool IsShirtLoaded
        {
            get { return (bool)GetValue(IsShirtLoadedProperty); }
            set { SetValue(IsShirtLoadedProperty, value); }
        }

        public static readonly DependencyProperty IsShirtLoadedProperty = DependencyProperty.Register("IsShirtLoaded", typeof(bool), typeof(ShirtSequenceControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsShirtLoadedChanged));

        private static void OnIsShirtLoadedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                // Shirt loaded --> Start animations
                ShirtSequenceControl control = d as ShirtSequenceControl;
                if (control != null)
                {
                    VisualStateManager.GoToState(control, "Next", true);
                }
            }
        }


        //public static ICommand LoadShirtCommand { get; set; }


        //public ICommand LoadShirtCommand
        //{
        //    get { return (ICommand)GetValue(LoadShirtCommandProperty); }
        //    set { SetValue(LoadShirtCommandProperty, value); }
        //}

        //public static readonly DependencyProperty LoadShirtCommandProperty = DependencyProperty.Register("LoadShirtCommand", typeof(ICommand), typeof(ShirtSequenceControl), new PropertyMetadata(LoadShirtCommandChanged));

        //private static void LoadShirtCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    // Code for dealing with your property changes
        //}

        public ICommand NextShirtCommand
        {
            get { return (ICommand)GetValue(NextShirtCommandProperty); }
            set { SetValue(NextShirtCommandProperty, value); }
        }

        public static readonly DependencyProperty NextShirtCommandProperty = DependencyProperty.Register("NextShirtCommand", typeof(ICommand), typeof(ShirtSequenceControl), new PropertyMetadata(NextShirtCommandChanged));

        private static void NextShirtCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Code for dealing with your property changes
        }


        public ICommand ShirtLoadedCommand
        {
            get { return (ICommand)GetValue(ShirtLoadedCommandProperty); }
            set { SetValue(ShirtLoadedCommandProperty, value); }
        }

        public static readonly DependencyProperty ShirtLoadedCommandProperty = DependencyProperty.Register("ShirtLoadedCommand", typeof(ICommand), typeof(ShirtSequenceControl), new PropertyMetadata(ShirtLoadedCommandChanged));

        private static void ShirtLoadedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Code for dealing with your property changes
        }


        public ShirtSequenceControl()
        {
            //LoadShirtCommand = new LoadShirtCommand(this);
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            GetStoryboard(this, "Next").Completed += storyboard_Completed;
        }


        private Storyboard GetStoryboard(FrameworkElement obj, String stateName)
        {
            if ((stateName == null) || (stateName.Equals("")) || (obj == null))
                throw new InvalidOperationException("Statename must contain a valid value.");

            var element = Template.FindName("PART_LayoutRoot", this) as FrameworkElement;
            var visualStateGroups = VisualStateManager.GetVisualStateGroups(element) as IList;

            foreach (VisualStateGroup group in visualStateGroups)
            {
                var states = @group.States as IList;
                foreach (VisualState state in states)
                {
                    if (state.Name.Equals(stateName))
                    {
                        return state.Storyboard;
                    }
                }
            }
            throw new InvalidOperationException("FrameworkElement " + obj.Name + " does not contain a Storyboard for VisualState " + stateName + ".");
        }



        private void storyboard_Completed(object sender, object e)
        {

            if (NextShirtCommand.CanExecute(null))
                NextShirtCommand.Execute(null);
            IsShirtLoaded = false;
            VisualStateManager.GoToState(this, "Jump", true);

        }
    }



    //public class LoadShirtCommand : ICommand
    //{
    //    public FrameworkElement AssociatedControl;

    //    public LoadShirtCommand(FrameworkElement associatedControl)
    //    {
    //        AssociatedControl = associatedControl;
    //    }

    //    public bool CanExecute(object parameter)
    //    {
    //        return true;
    //    }

    //    public event EventHandler CanExecuteChanged;

    //    public void Execute(object parameter)
    //    {
    //        VisualStateManager.GoToState((Control)AssociatedControl, "Next", true);
            
    //    }
    //}
}
