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


using System.ComponentModel;
using SoftArcs.WPFSmartLibrary.CommonAnimations;

namespace SoftArcs.WPFSmartLibrary.UIClassAttachedProperties
{
	using System;
	using System.Windows;
	using System.Windows.Data;
	using System.Windows.Media.Animation;

	/// <summary>
	/// Supplies attached properties that provides animation to the Visibility property.
	/// </summary>
	public class VisibilityAnimation
	{
		#region Fields

		private const double STANDARD_ANIMATION_DURATION = 350.0;

		#endregion // Fields

		#region Enumerations

		/// <summary>
		/// The type of the animation.
		/// </summary>
		public enum AnimationType
		{
			/// <summary>
			/// No animation.
			/// </summary>
			None,
			/// <summary>
			/// Fade in / Fade out.
			/// </summary>
			Fade
		}

		#endregion

		#region Constructor

		/// <summary>
		/// VisibilityAnimation static constructor.
		/// </summary>
		static VisibilityAnimation()
		{
			// Here we "register" on the Visibility property the "before change" event (OLD version)
			//UIElement.VisibilityProperty.AddOwner( typeof( FrameworkElement ),
			//				new FrameworkPropertyMetadata( Visibility.Visible, VisibilityChanged, CoerceVisibility ) );

			// Here we "register" on the Visibility property the "before change" event (NEW version)
			var descriptor = DependencyPropertyDescriptor.FromProperty( UIElement.VisibilityProperty, typeof( FrameworkElement ) );
			descriptor.DesignerCoerceValueCallback = CoerceVisibility;
		}

		#endregion // Constructor

		#region AnimationType - Attached Dependency Property

		/// <summary>
		/// Using a DependencyProperty as the backing store for AnimationType.  
		/// This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty AnimationTypeProperty =
					DependencyProperty.RegisterAttached( "AnimationType", typeof( AnimationType ), typeof( VisibilityAnimation ),
							new FrameworkPropertyMetadata( AnimationType.None, OnAnimationTypePropertyChanged ) );
		/// <summary>
		/// Gets 'AnimationType' attached property.
		/// </summary>
		public static AnimationType GetAnimationType(DependencyObject dpo)
		{
			return (AnimationType)dpo.GetValue( AnimationTypeProperty );
		}

		/// <summary>
		/// Sets 'AnimationType' attached property.
		/// </summary>
		public static void SetAnimationType(DependencyObject dpo, AnimationType value)
		{
			dpo.SetValue( AnimationTypeProperty, value );
		}

		/// <summary>
		/// Handles changes to the 'AnimationType' attached property.
		/// </summary>
		private static void OnAnimationTypePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var frameworkElement = sender as FrameworkElement;

			if (frameworkElement != null)
			{
				// If AnimationType is set to True on this framework element, 
				if (GetAnimationType( frameworkElement ) != AnimationType.None)
				{
					// Add this framework element to hooked list ...
					AnimationObserver.MonitorElement( frameworkElement );
				}
				else
				{
					// ... otherwise, remove it from the hooked list
					AnimationObserver.ReleaseElement( frameworkElement );
				}
			}
		}

		#endregion

		#region IgnoreFirstTime - Attached Dependency Property

		/// <summary>
		/// Using a DependencyProperty as the backing store for IgnoreFirstTime.  
		/// This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty IgnoreFirstTimeProperty =
					DependencyProperty.RegisterAttached( "IgnoreFirstTime", typeof( bool ), typeof( VisibilityAnimation ),
																	 new UIPropertyMetadata( false ) );
		/// <summary>
		/// Gets 'IgnoreFirstTime' attached property.
		/// </summary>
		public static bool GetIgnoreFirstTime(DependencyObject dpo)
		{
			return (bool)dpo.GetValue( IgnoreFirstTimeProperty );
		}

		/// <summary>
		/// Sets 'IgnoreFirstTime' attached property.
		/// </summary>
		public static void SetIgnoreFirstTime(DependencyObject dpo, bool value)
		{
			dpo.SetValue( IgnoreFirstTimeProperty, value );
		}

		#endregion // IgnoreFirstTime - Attached Dependency Property

		#region AnimationDuration - Attached Dependency Property

		/// <summary>
		/// Using a DependencyProperty as the backing store for IgnoreFirstTime.  
		/// This enables animation, styling, binding, etc...
		/// </summary>
		public static readonly DependencyProperty AnimationDurationProperty =
					DependencyProperty.RegisterAttached( "AnimationDuration", typeof( double ), typeof( VisibilityAnimation ),
																	 new UIPropertyMetadata( STANDARD_ANIMATION_DURATION ) );

		/// <summary>
		/// Gets 'AnimationDuration' attached property.
		/// </summary>
		public static double GetAnimationDuration(DependencyObject dpo)
		{
			return (double)dpo.GetValue( AnimationDurationProperty );
		}

		/// <summary>
		/// Sets 'AnimationDuration' attached property.
		/// </summary>
		public static void SetAnimationDuration(DependencyObject dpo, double value)
		{
			dpo.SetValue( AnimationDurationProperty, value );
		}

		#endregion // AnimationDuration - Attached Dependency Property

		#region Visibility changed handling

		/// <summary>
		/// Visibility changed.
		/// </summary>
		private static void VisibilityChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			// Ignore
		}

		/// <summary>
		/// Coerce visibility.
		/// </summary>
		private static object CoerceVisibility(DependencyObject dependencyObject, object baseValue)
		{
			// Make sure the DependencyObject is a FrameworkElement
			var frameworkElement = dependencyObject as FrameworkElement;

			if (frameworkElement == null)
			{
				return baseValue;
			}

			// Cast to type safe value
			var visibility = (Visibility)baseValue;

			// If Visibility value hasn't change, do nothing. This can happen if the Visibility property is set
			// using data binding and the binding source has changed but the new visibility value hasn't changed.
			if (visibility == frameworkElement.Visibility)
			{
				return baseValue;
			}

			// If element is not hooked by our attached property, stop here
			if (AnimationObserver.IsMonitoredElement( frameworkElement ) == false)
			{
				return baseValue;
			}

			// If element has IgnoreFirstTime flag set, then ignore the first time the property is coerced.
			if (GetIgnoreFirstTime( frameworkElement ) == true)
			{
				SetIgnoreFirstTime( frameworkElement, false );

				return baseValue;
			}

			// Update animation flag. If animation already started - don't restart it (otherwise, infinite loop)
			if (AnimationObserver.ReverseAnimationRunningFlag( frameworkElement ) == true)
			{
				return baseValue;
			}

			// If we get here, it means we have to start fade in or fade out animation. In any case return value
			// of this method will be Visibility.Visible, to allow the animation.
			var doubleAnimation = new DoubleAnimation
			{
				Duration = new Duration( TimeSpan.FromMilliseconds( GetAnimationDuration( frameworkElement ) ) )
			};

			// When animation completes, set the visibility value to the requested value (baseValue)
			doubleAnimation.Completed += (sender, eventArgs) =>
					{
						if (visibility == Visibility.Visible)
						{
							// In case we change into Visibility.Visible, the correct value is already set.
							// So just update the animation started flag
							AnimationObserver.ReverseAnimationRunningFlag( frameworkElement );
						}
						else
						{
							// This will trigger value coercion again but ReverseAnimationRunningFlag() function will
							// return true this time, thus animation will not be triggered. 
							if (BindingOperations.IsDataBound( frameworkElement, UIElement.VisibilityProperty ))
							{
								// Set visiblity using bounded value
								var bindingValue = BindingOperations.GetBinding( frameworkElement, UIElement.VisibilityProperty );

							    if (bindingValue == null)
							        return;


								BindingOperations.SetBinding( frameworkElement, UIElement.VisibilityProperty, bindingValue );
							}
							else
							{
								// No binding, just assign the value
								frameworkElement.Visibility = visibility;
							}
						}
					};

			doubleAnimation.To = visibility == Visibility.Visible ? 1.0 : 0.0;

			// Start animation
			frameworkElement.BeginAnimation( UIElement.OpacityProperty, doubleAnimation );

			// Make sure the element remains visible during the animation. The original requested value will be
			// set in the completed event of the animation
			return Visibility.Visible;
		}

		#endregion // Visibility changed handling
	}
}
