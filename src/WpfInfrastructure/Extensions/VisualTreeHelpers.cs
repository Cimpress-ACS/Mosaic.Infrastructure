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


namespace VP.FF.PT.Common.WpfInfrastructure.Extensions
{
	using System.Windows;
	using System.Windows.Media;

	public static class VisualTreeHelpers
	{
		/// <summary>
		/// Finds a Child of a given item in the visual tree. 
		/// </summary>
		/// <param name="parent">A direct parent of the queried item.</param>
		/// <typeparam name="T">The type of the queried item.</typeparam>
		/// <param name="childName">x:Name or Name of child. </param>
		/// <returns>The first parent item that matches the submitted type parameter. 
		/// If not matching item can be found, 
		/// a null parent is being returned.</returns>
		public static T FindChild<T>(DependencyObject parent, string childName)
			where T : DependencyObject
		{
			// Confirm parent and childName are valid. 
			if (parent == null) return null;

			T foundChild = null;

			var childrenCount = VisualTreeHelper.GetChildrenCount( parent );
			for (var i = 0; i < childrenCount; i++)
			{
				var child = VisualTreeHelper.GetChild( parent, i );

				// If the child is not of the request child type child
				var childType = child as T;
				if (childType == null)
				{
					// recursively drill down the tree
					foundChild = FindChild<T>( child, childName );

					// If the child is found, break so we do not overwrite the found child. 
					if (foundChild != null)
					{
						break;
					}
				}
				else if (!string.IsNullOrEmpty( childName ))
				{
					var frameworkElement = child as FrameworkElement;

					// If the child's name is set for search
					if (frameworkElement != null && frameworkElement.Name == childName)
					{
						// if the child's name is of the request name
						foundChild = (T)child;
						break;
					}
				}
				else
				{
					// child element found.
					foundChild = (T)child;
					break;
				}
			}

			return foundChild;
		}

		/// <summary>
		/// Finds the ancestor or self.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">The object.</param>
		/// <returns>``0.</returns>
		public static T FindAncestorOrSelf<T>(DependencyObject obj)
			where T : DependencyObject
		{
			while (obj != null)
			{
				var objTest = obj as T;

				if (objTest != null) return objTest;

				obj = GetParent( obj );
			}

			return null;
		}

		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns>DependencyObject.</returns>
		public static DependencyObject GetParent(DependencyObject obj)
		{
			if (obj == null) return null;

			var ce = obj as ContentElement;

			if (ce != null)
			{
				var parent = ContentOperations.GetParent( ce );

				if (parent != null) return parent;

				var fce = ce as FrameworkContentElement;

				return fce != null ? fce.Parent : null;
			}

			return VisualTreeHelper.GetParent( obj );
		}

		/// <summary>
		/// Finds a parent of a given item on the visual tree.
		/// </summary>
		/// <typeparam name="T">The type of the queried item.</typeparam>
		/// <param name="child">A direct or indirect child of the
		/// queried item.</param>
		/// <returns>The first parent item that matches the submitted
		/// type parameter. If not matching item can be found, a null
		/// reference is being returned.</returns>
		public static T TryFindParent<T>(DependencyObject child)
			 where T : DependencyObject
		{
			//get parent item
			var parentObject = GetParentObject( child );

			//we've reached the end of the tree
			if (parentObject == null) return null;

			//check if the parent matches the type we're looking for
			var parent = parentObject as T;

			//use recursion to proceed with next level
			return parent ?? TryFindParent<T>( parentObject );
		}

		/// <summary>
		/// This method is an alternative to WPF's
		/// <see cref="VisualTreeHelper.GetParent"/> method, which also
		/// supports content elements. Keep in mind that for content element,
		/// this method falls back to the logical tree of the element!
		/// </summary>
		/// <param name="child">The item to be processed.</param>
		/// <returns>The submitted item's parent, if available. Otherwise
		/// null.</returns>
		public static DependencyObject GetParentObject(DependencyObject child)
		{
			if (child == null) return null;

			//handle content elements separately
			var contentElement = child as ContentElement;
			if (contentElement != null)
			{
				var parent = ContentOperations.GetParent( contentElement );
				if (parent != null) return parent;

				var fce = contentElement as FrameworkContentElement;
				return fce != null ? fce.Parent : null;
			}

			//also try searching for parent in framework elements (such as DockPanel, etc)
			var frameworkElement = child as FrameworkElement;
			if (frameworkElement != null)
			{
				var parent = frameworkElement.Parent;
				if (parent != null) return parent;
			}

			//if it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
			return VisualTreeHelper.GetParent( child );
		}
	}
}
