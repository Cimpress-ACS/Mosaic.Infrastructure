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


namespace SoftArcs.WPFSmartLibrary.CommonAnimations
{
	using System.Windows;
	using System.Collections.Generic;

	/// <summary>
	/// Provides a set of methods for monitoring animated <see cref="DependencyObject"/> objects.
	/// </summary>
	internal static class AnimationObserver
	{
		private static readonly Dictionary<DependencyObject, bool> monitoredElements = new Dictionary<DependencyObject, bool>();

		/// <summary>
		/// Add a <see cref="DependencyObject"/> object to the observed dictionary.
		/// </summary>
		/// <param name="element">The <see cref="DependencyObject"/> element.</param>
		internal static void MonitorElement(DependencyObject element)
		{
			monitoredElements.Add( element, false );
		}

		/// <summary>
		/// Remove a <see cref="DependencyObject"/> object from the observed dictionary.
		/// </summary>
		/// <param name="element">The <see cref="DependencyObject"/> element.</param>
		internal static void ReleaseElement(DependencyObject element)
		{
			if (monitoredElements.ContainsKey( element ))
			{
				monitoredElements.Remove( element );
			}
		}

		/// <summary>
		/// Check whether the given <see cref="DependencyObject"/> object is being observed or not.
		/// </summary>
		/// <param name="element">The <see cref="DependencyObject"/> element.</param>
		/// <returns>True if the element is already being monitored.</returns>
		internal static bool IsMonitoredElement(DependencyObject element)
		{
			return monitoredElements.ContainsKey( element );
		}

		/// <summary>
		/// Check whether a animation is currently running on the given <see cref="DependencyObject"/> object.
		/// </summary>
		/// <param name="element">The <see cref="DependencyObject"/> element.</param>
		/// <returns>True if the element has a running animation.</returns>
		internal static bool IsAnimationRunning(DependencyObject element)
		{
			return monitoredElements[element];
		}

		/// <summary>
		/// Reverse the animation flag for the given <see cref="DependencyObject"/> object.
		/// </summary>
		/// <param name="element">The <see cref="DependencyObject"/> element.</param>
		/// <returns>The old state of the animation flag.</returns>
		internal static bool ReverseAnimationRunningFlag(DependencyObject element)
		{
			var animationStarted = monitoredElements[element];

			monitoredElements[element] = !animationStarted;

			return animationStarted;
		}
	}
}
