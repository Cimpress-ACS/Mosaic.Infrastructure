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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VP.FF.PT.Common.WpfInfrastructure.Extensions;

namespace VP.FF.PT.Common.WpfInfrastructure.Controls.AnimatedSlotPanel
{
	/// <summary>
	/// A custom panel with slots of fixed size and animation capabilities
	/// (This panel can be used as an ItemHost in an ItemsPanelTemplate)
	/// </summary>
	public class AnimatedSlotPanel : Panel
	{
		#region Constants

		private const double EPSILON = 0.1;

		#endregion

		#region Private Fields

		private readonly Collection<UIElementWithMetaData> internalChildCollection;

		private bool newChildAdded;
		private bool resizeInProgress;
		private bool initialSizeChangeInProgress;
		private bool changeOfPanelHeightInProgress;

		private Size previousSize;

		private TimeSpan arrangementTimeSpan;

		private UIElement addedUIElement;
		private UIElement removedUIElement;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AnimatedSlotPanel"/> class.
		/// </summary>
		public AnimatedSlotPanel()
		{
			internalChildCollection = new Collection<UIElementWithMetaData>();
			
			//this.SizeChanged += AnimatedSlotPanel_SizeChanged;
		}

		#endregion

		#region Dependency Properties

		/// <summary>
		/// The number of available columns (default: 4). This is a dependency property.
		/// </summary>
		public int ColumnCount
		{
			get { return (int)GetValue( ColumnCountProperty ); }
			set { SetValue( ColumnCountProperty, value ); }
		}

		/// <summary>
		/// The number of available columns (default: 4).
		/// </summary>
		public static readonly DependencyProperty ColumnCountProperty = DependencyProperty.Register(
			 "ColumnCount",
			 typeof( int ),
			 typeof( AnimatedSlotPanel ),
			 new FrameworkPropertyMetadata(
				  11,
				  FrameworkPropertyMetadataOptions.AffectsMeasure,
				  (dpo, ea) => ((AnimatedSlotPanel)dpo).OnColumnCountChanged( (int)ea.NewValue ) ) );

		private void OnColumnCountChanged(int newValue)
		{
			if (newValue == 0)
			{
				// If we have zero columns (i.e. binding mode), then assign the  DependencyProperty default value
				var metadata = ColumnCountProperty.GetMetadata( typeof( AnimatedSlotPanel ) );
				ColumnCount = (int)metadata.DefaultValue;
			}
			// Simulate a resize operation to prevent unpredictable animations 
			resizeInProgress = true;
		}

		/// <summary>
		/// The number of available rows (default: 4). This is a dependency property.
		/// </summary>
		public int RowCount
		{
			get { return (int)GetValue( RowCountProperty ); }
			set { SetValue( RowCountProperty, value ); }
		}

		/// <summary>
		/// The number of available rows (default: 4).
		/// </summary>
		public static readonly DependencyProperty RowCountProperty = DependencyProperty.Register(
			 "RowCount",
			 typeof( int ),
			 typeof( AnimatedSlotPanel ),
			 new FrameworkPropertyMetadata(
				  5,
				  FrameworkPropertyMetadataOptions.AffectsMeasure,
				  (dpo, ea) => ((AnimatedSlotPanel)dpo).OnRowCountChanged( (int)ea.NewValue ) ) );

		private void OnRowCountChanged(int newValue)
		{
			if (newValue == 0)
			{
				// If we have zero rows (i.e. binding mode), then assign the  DependencyProperty default value
				var metadata = RowCountProperty.GetMetadata( typeof( AnimatedSlotPanel ) );
				RowCount = (int)metadata.DefaultValue;
			}
			// Simulate a resize operation to prevent unpredictable animations 
			resizeInProgress = true;
		}

		/// <summary>
		/// The duration of the movement animations (default: 700 milliseconds). This is a dependency property.
		/// </summary>
		[TypeConverter( typeof( DurationConverter ) )]
		public Duration MovementAnimationDuration
		{
			get { return (Duration)GetValue( MovementAnimationDurationProperty ); }
			set { SetValue( MovementAnimationDurationProperty, value ); }
		}

		/// <summary>
		/// The duration of the movement animations (default: 700 milliseconds).
		/// </summary>
		public static readonly DependencyProperty MovementAnimationDurationProperty = DependencyProperty.Register(
			 "MovementAnimationDuration",
			 typeof( Duration ),
			 typeof( AnimatedSlotPanel ),
			 new FrameworkPropertyMetadata( new Duration( TimeSpan.FromMilliseconds( 700.0 ) ) ) );

		/// <summary>
		/// The delay of the merge animations (default: 600 milliseconds). This is a dependency property. 
		/// </summary>
		[TypeConverter( typeof( DurationConverter ) )]
		public Duration MergeAnimationDelay
		{
			get { return (Duration)GetValue( MergeAnimationDelayProperty ); }
			set { SetValue( MergeAnimationDelayProperty, value ); }
		}

		/// <summary>
		/// The delay of the merge animations (default: 600 milliseconds).
		/// </summary>
		public static readonly DependencyProperty MergeAnimationDelayProperty = DependencyProperty.Register(
			 "MergeAnimationDelay",
			 typeof( Duration ),
			 typeof( AnimatedSlotPanel ),
			 new FrameworkPropertyMetadata( new Duration( TimeSpan.FromMilliseconds( 600.0 ) ) ) );

		#endregion

		#region Measure and Arrangement Overrides

		/// <summary>
		/// Overrides the standard MeasureOverride method from the <see cref="System.Windows.FrameworkElement" /> class.
		/// </summary>
		/// <param name="availableSize">The available size that this element can give to child elements. Infinity can be specified as a value to indicate that the element will size to whatever content is available.</param>
		/// <returns>
		/// The size that this element determines it needs during layout, based on its calculations of child element sizes.
		/// </returns>
		protected override Size MeasureOverride(Size availableSize)
		{
			// [HV] Don't do this because of possible performance and unpredictable animation issues (see my new solution below)
			// (Centigrade IW)The internal collection needs to be cleared everytime,
			// to avoid the accumulation of old items, when the source is reset.
			//if (Children.Count == 0)
			//{
			//internalChildCollection.Clear();
			//}

			// If the source collection has been cleared or the ItemsSource has changed, clear also our internal collection
			if (Children.Count == 0 || !internalChildCollection.Any( x => Children.Contains( x.Child ) ))
			{
				internalChildCollection.Clear();
			}

			// If a child was removed ...
			if (removedUIElement != null)
			{
				// ... try to find the removed child in our internal child collection
				var existingChild = internalChildCollection.FirstOrDefault( item => item.Child.Equals( removedUIElement ) );

				// If the child was found ...
				if (existingChild != null)
				{
					// ... update the state of our internal child
					existingChild.IsRemoved = true;
					existingChild.IsRecentlyAdded = false;
				}
			}

			// Calculate the width and height of every single Slot
			var slotWidth = availableSize.Width / ColumnCount;
			var slotHeight = availableSize.Height / RowCount;

			// Iterate through all childs in the UIElementCollection (except null objects)
			foreach (var child in Children.Cast<UIElement>().Where( child => child != null ))
			{
				child.Measure( new Size( slotWidth, slotHeight ) );
			}

			if (this.previousSize != availableSize)
			{
				resizeInProgress = true;
				initialSizeChangeInProgress = this.previousSize.Equals( new Size( 0, 0 ) );
			}
			this.previousSize = availableSize;

			return availableSize;
		}

		/// <summary>
		/// Overrides the standard ArrangeOverride method from the <see cref="System.Windows.FrameworkElement" /> class.
		/// </summary>
		/// <param name="finalSize">The final area within the parent that this element should use to arrange itself and its children.</param>
		/// <returns>
		/// The actual size used.
		/// </returns>
		protected override Size ArrangeOverride(Size finalSize)
		{
			// If an item was removed don't move resp. animate anything
			if (removedUIElement != null)
			{
				return finalSize;
			}

			// If a row was added and as a result of this the panel height has changed don't move resp. animate anything
			if (changeOfPanelHeightInProgress)
			{
				changeOfPanelHeightInProgress = false;
				return finalSize;
			}

			// Set the time stamp at the beginning of the arrangement process
			var startArrangementTimeStamp = DateTime.Now.Ticks;

			// Iterate through all childs in the UIElementCollection
			for (var i = 0; i < Children.Count; i++)
			{
				// Get the current child by it's index
				var child = Children[i];
				if (child == null)
				{
					continue;
				}

				// Try to find the current child in our internal child collection
				var existingChild = internalChildCollection.FirstOrDefault( item => item.Child.Equals( child ) );
				if (existingChild == null)
				{
					// If the current child is not in our internal child collection, then add it to our internal collection
					internalChildCollection.Add( new UIElementWithMetaData
						 {
							 Child = child,
							 ChildIndex = i,
							 IsRecentlyAdded = true
						 } );
				}
				else
				{
					existingChild.IsRecentlyAdded = false;
				}
			}

			// Calculate the width and height of every single Slot
			var slotWidth = finalSize.Width / ColumnCount;
			var slotHeight = finalSize.Height / RowCount;
			// Calculate the initial horizontal and vertical offset
			var horizontalOffset = slotWidth * (ColumnCount - 1);
			// ReSharper disable once PossibleLossOfFraction
			var verticalOffset = slotHeight * (internalChildCollection.Count / ColumnCount);
			// Determine if the first column is full
			var isFirstColumnFull = (internalChildCollection.Count % ColumnCount).Equals( 0 );

			// This is our column counter
			var currentColumn = ColumnCount;

			// Here we hold all indexes of childs which are involved in the merge process
			var currentRowMergeIndexes = new List<int>();
			var previousRowMergeIndexes = new List<int>();

			// Count of row pairs to merge ( 2 rows -> 1 row )
			var countOfRowPairs = 0;

			// Count of deleted childs in curent row pair, so we can skip all childs after ColumnColunt childs were removed
			var countOfDeletedChildsInCurrentRowPair = 0;

			// If a new child was added and the first column is full ...
			if (newChildAdded && isFirstColumnFull)
			{
				// ... iterate through all rows
				for (var i = 0; i < internalChildCollection.Count; i += ColumnCount)
				{
					var removedChildsInCurrentRow = 0;
					var removedChildsInPreviousRow = 0;

					// Determine the first and last child indexes of the current and previous row
					var firstChildIndexInCurrentRow = i - (i % ColumnCount);
					var lastChildIndexInCurrentRow = firstChildIndexInCurrentRow + ColumnCount - 1;
					var firstChildIndexInPreviousRow = firstChildIndexInCurrentRow + ColumnCount;
					var lastChildIndexInPreviousRow = lastChildIndexInCurrentRow + ColumnCount;

					// Count all removed childs in the current row
					for (var childIndex = firstChildIndexInCurrentRow; childIndex <= lastChildIndexInCurrentRow; childIndex++)
					{
						if (childIndex >= internalChildCollection.Count)
						{
							continue;
						}

						if (internalChildCollection[childIndex].IsRemoved)
						{
							removedChildsInCurrentRow++;
						}
					}

					// Count all removed childs in the previous row
					for (var childIndex = firstChildIndexInPreviousRow; childIndex <= lastChildIndexInPreviousRow; childIndex++)
					{
						if (childIndex >= internalChildCollection.Count)
						{
							continue;
						}

						if (internalChildCollection[childIndex].IsRemoved)
						{
							removedChildsInPreviousRow++;
						}
					}

					// If all removed childs in the current row and the previous row are greater or equal to the column count ...
					if (removedChildsInCurrentRow + removedChildsInPreviousRow >= ColumnCount)
					{
						// ... add all child indexes of the current and previous row to our index lists
						currentRowMergeIndexes.AddRange( Enumerable.Range( firstChildIndexInCurrentRow, ColumnCount ) );
						previousRowMergeIndexes.AddRange( Enumerable.Range( firstChildIndexInPreviousRow, ColumnCount ) );

						countOfRowPairs++;
						i += ColumnCount;
					}
				}

				// Update the initial vertical offset considering the count of row pairs
				if (countOfRowPairs > 0)
				{
					verticalOffset -= slotHeight * countOfRowPairs;
				}
			}

			// Iterate through all childs in our internal child collection
			for (var i = 0; i < internalChildCollection.Count; i++)
			{
				// Get the current internal child by it's index
				var existingChild = internalChildCollection[i];
				var child = existingChild.Child;
				if (child == null)
				{
					continue;
				}

				// Get the current TransformGroup
				var existingTransformGroup = child.RenderTransform as TransformGroup;

				var translateTransform = new TranslateTransform();
				var skewTransform = new SkewTransform();
				var transformGroup = new TransformGroup();

				// If there is no current TransformGroup, create a new one ... 
				if (existingTransformGroup == null)
				{
					transformGroup.Children.Add( translateTransform );
					transformGroup.Children.Add( skewTransform );
					child.RenderTransform = transformGroup;
				}
				else
				{
					// ... else retrieve the available Transform objects from the existing TransformGroup
					translateTransform = existingTransformGroup.Children[0] as TranslateTransform;
					skewTransform = existingTransformGroup.Children[1] as SkewTransform;
				}

				// First arrangement of the child on position [0,0] with the size of a single Slot
				// The animation values will overwrite this first position settings
				child.Arrange( new Rect( 0, 0, slotWidth, slotHeight ) );

				// New childs will slide in from outside ...
				var fromHorizontalPosition = -slotWidth;
				var fromVerticalPosition = 0.0;

				// If the current child wasn't recently added and the initial size change is not in progress ...
				if (!internalChildCollection[i].IsRecentlyAdded && !initialSizeChangeInProgress)
				{
					// ...  retrieve the current position as the animation start position
					fromHorizontalPosition = existingChild.CurrentPosition.X;
					fromVerticalPosition = existingChild.CurrentPosition.Y;
				}

				// Update the current position data and the animation progress state
				existingChild.CurrentPosition = new Point( horizontalOffset, verticalOffset );
				existingChild.HorizontalAnimationInProgress = true;

				var mergeInProgress = false;
				var lastChildIndexInCurrentRow = -1;

				if (translateTransform != null)
				{
					// Animate the vertical position of the current child from the childs current vertical position
					// to the calculated vertical offset
					var verticalAnimation = new DoubleAnimation( fromVerticalPosition, verticalOffset, MovementAnimationDuration )
					{
						AccelerationRatio = 0.5,
						DecelerationRatio = 0.3
					};

					// If the first column is full ...
					if (newChildAdded && isFirstColumnFull)
					{
						// ... merge corresponding rows, where the cumulated item count is equal to the column count
						var removedChildsInCurrentRow = 0;
						// var removedChildsInNextRow = 0;

						// Determine the first and last child indexes of the current and next row
						var firstChildIndexInCurrentRow = i - (i % ColumnCount);
						lastChildIndexInCurrentRow = firstChildIndexInCurrentRow + ColumnCount - 1;
						// var firstChildIndexInNextRow = firstChildIndexInCurrentRow - ColumnCount;
						// var lastChildIndexInNextRow = lastChildIndexInCurrentRow - ColumnCount;

						// Count all removed childs in the current row
						for (var childIndex = firstChildIndexInCurrentRow; childIndex <= lastChildIndexInCurrentRow; childIndex++)
						{
							if (childIndex >= internalChildCollection.Count)
							{
								continue;
							}

							if (internalChildCollection[childIndex].IsRemoved)
							{
								removedChildsInCurrentRow++;
							}
						}

						// Count all removed childs in the next row
						// for (var childIndex = firstChildIndexInNextRow; childIndex <= lastChildIndexInNextRow; childIndex++)
						// {
						// if (childIndex < 0 || childIndex >= internalChildCollection.Count) continue;
						// if (internalChildCollection[childIndex].IsRemoved) removedChildsInNextRow++;
						// }

						// If the current child is located in a row to merge
						if (currentRowMergeIndexes.Contains( i ) || previousRowMergeIndexes.Contains( i ))
						{
							mergeInProgress = true;

							// If the child is marked as removed ...
							if (existingChild.IsRemoved)
							{
								// ... update the horizontal offset
								horizontalOffset += slotWidth;
								// ... and mark the child for deletion
								// (only if all deleted childs in the current row pair don't exceed the ColumnCount)
								if (++countOfDeletedChildsInCurrentRowPair <= ColumnCount)
								{
									existingChild.IsMarkedForDeletion = true;
								}
							}

							// If the child is the first child in the previous row ...
							if (previousRowMergeIndexes.Contains( i ) && i == firstChildIndexInCurrentRow)
							{
								// ... update the horizontal offset
								horizontalOffset -= slotWidth * removedChildsInCurrentRow;
								// horizontalOffset -= slotWidth * (ColumnCount - removedChildsInPreviousRow);
							}
						}
						else
						{
							lastChildIndexInCurrentRow = -1;
							countOfDeletedChildsInCurrentRowPair = 0;
						}

						// Start the movement of all lines AFTER the sliding movement and the arrangement is finished
						verticalAnimation.BeginTime = MovementAnimationDuration.TimeSpan + arrangementTimeSpan;

						// If there are rows to merge ..
						if (countOfRowPairs > 0)
						{
							// ... add the merge animation delay to the vertical animation delay
							verticalAnimation.BeginTime += MergeAnimationDelay.TimeSpan;
						}
					}

					// Hook the vertical animation completed event, so we can remove the childs which are marked for deletion
					verticalAnimation.Completed += (os, ea) => verticalAnimation_Completed( child );

					// Apply the vertical animation ONLY if the vertical position has changed
					if (!fromVerticalPosition.Equals( verticalOffset ))
					{
						// !-----------------------------------------------------------------------------------------------------------------
						Debug.WriteLine( "Vertical Movement Item No. {0} from {1} to {2} ", i, fromVerticalPosition, verticalOffset );
						// !-----------------------------------------------------------------------------------------------------------------
						translateTransform.BeginAnimation( TranslateTransform.YProperty, verticalAnimation, HandoffBehavior.Compose );
					}

					// Animate the horizontal position of the current child from the childs current horizontal position
					// to the calculated horizontal offset
					var horizontalAnimation = new DoubleAnimation( fromHorizontalPosition, horizontalOffset, MovementAnimationDuration )
					{
						AccelerationRatio = 0.5,
						DecelerationRatio = 0.3
					};
					// Hook the animation completed event, so we can change the "HorizontalAnimationInProgress" flag
					horizontalAnimation.Completed += (os, ea) => horizontalAnimation_Completed( child );

					if (internalChildCollection[i].IsRecentlyAdded || existingChild.HorizontalAnimationInProgress == false ||
						  resizeInProgress || mergeInProgress)
					{
						// !-----------------------------------------------------------------------------------------------------------------
						Debug.WriteLine( "Horizontal Movement Item No. " + i );
						Debug.WriteLine( "Arrange time span : " + arrangementTimeSpan.TotalMilliseconds );
						// !-----------------------------------------------------------------------------------------------------------------

						// Set the beginning of the horizontal animation to the end of the arrangement process
						// (this will prevent unpredictable animation effects)
						horizontalAnimation.BeginTime = arrangementTimeSpan;

						// If a merge is in progress..
						if (mergeInProgress)
						{
							// ... add the merge animation delay to the horizontal animation
							horizontalAnimation.BeginTime += MergeAnimationDelay.TimeSpan;

							// If we have the last child of the current merged row reset the horizontal offset to EPSILON
							if (currentRowMergeIndexes.Contains( i ) && i == lastChildIndexInCurrentRow)
							{
								horizontalOffset = EPSILON;
							}
						}
						else
						{
							// This is neccessary to move the child to it's current horizontal start position 
							var moveToStartPositionAnimation = new DoubleAnimation( fromHorizontalPosition, fromHorizontalPosition, new Duration( new TimeSpan( 0 ) ) );
							translateTransform.BeginAnimation( TranslateTransform.XProperty, moveToStartPositionAnimation, HandoffBehavior.Compose );
						}

						translateTransform.BeginAnimation( TranslateTransform.XProperty, horizontalAnimation, HandoffBehavior.Compose );
					}
				}

				existingChild.CurrentPosition = new Point( horizontalOffset, verticalOffset );

				//// If the child is the last child in the current merged row ...
				// if (currentRowMergeIndexes.Contains( i ) && i == lastChildIndexInCurrentRow)
				// // ... update the vertical offset
				// verticalOffset += slotHeight;

				// If the current child is the first in the row, then update the vertical and horizontal offset for the next childs
				// and set the current column to the last column in the row
				if (horizontalOffset <= EPSILON)
				{
					// If the child is not the last child in a row that is currently merged ...
					if (!(currentRowMergeIndexes.Contains( i ) && i == lastChildIndexInCurrentRow))
					{
						verticalOffset -= slotHeight; // ... update the vertical offset for the next child
					}

					horizontalOffset = slotWidth * (ColumnCount - 1); // Update the horizontal offset for the next child
					currentColumn = ColumnCount; // Reset our column counter to last column in row
				}
				else
				{
					// If we have the recently added child ...
					if (newChildAdded && child.Equals( addedUIElement ) && skewTransform != null)
					{
						// ... perform an additional skew animation
						// Calculate the skew angle dependent on the distance to the end of line (base angle = -30°)
						var skewAngle = -30.0 * ((double)currentColumn / ColumnCount);
						var skewAnimation = new DoubleAnimationUsingKeyFrames
						{
							Duration = MovementAnimationDuration,
							BeginTime = arrangementTimeSpan
						};
						var fractionOfMovementAnimationDuration = (MovementAnimationDuration.TimeSpan.Multiply( 0.65 ));

						skewAnimation.KeyFrames.Add( new LinearDoubleKeyFrame( skewAngle, // Target value (KeyValue)
																														 KeyTime.FromTimeSpan( fractionOfMovementAnimationDuration ) ) // KeyTime
																		);
						skewAnimation.KeyFrames.Add( new LinearDoubleKeyFrame( 0, // Target value (KeyValue)
																														 KeyTime.FromTimeSpan( MovementAnimationDuration.TimeSpan ) ) // KeyTime
																		);

						skewTransform.BeginAnimation( SkewTransform.AngleXProperty, skewAnimation, HandoffBehavior.Compose );

						newChildAdded = false;
					}

					horizontalOffset -= slotWidth; // Update the horizontal offset for the next child
					currentColumn--; // Increase our column counter
				}
			}

			// Set the time stamp at the end of the arrangement process
			var endArrangementTimeStamp = DateTime.Now.Ticks;
			// Calculate the time span of the arrangement process
			arrangementTimeSpan = new TimeSpan( endArrangementTimeStamp - startArrangementTimeStamp );

			// Calculate the required rows (with respect to the count of childs marked for deletion)
			var countOfChildsMarkedForDeletion = internalChildCollection.Count( item => item.IsMarkedForDeletion );

			// (Centigrade IW) not used because the calculation is wrong, when the panel is resized to fullscreen
			var requiredRows = (internalChildCollection.Count - countOfChildsMarkedForDeletion) / ColumnCount + 1;
			//If we need more rows ...
			if (requiredRows > RowCount)
			{
				// ... increase the RowCount ...
				RowCount += requiredRows - RowCount;
				// ... and adjust the height of the panel itself
				this.Height = requiredRows * slotHeight;
				changeOfPanelHeightInProgress = true;
			}

			// Fill all the space given
			return finalSize;
		}

		#region Measure and Arrangement Event Handler

		void horizontalAnimation_Completed(object sender)
		{
			// Change the animation state of the current child
			var currentChild = internalChildCollection.FirstOrDefault( item => item.Child.Equals( sender ) );
			if (currentChild != null) currentChild.HorizontalAnimationInProgress = false;
		}

		private void verticalAnimation_Completed(object sender)
		{
			// Delete all marked childs form our internal collection			
			var childsMarkedForDeletion = internalChildCollection.Where( item => item.IsMarkedForDeletion );
			internalChildCollection.RemoveItems( childsMarkedForDeletion );

			// Renumerate all childs in our internal collection
			for (var i = 0; i < internalChildCollection.Count; i++)
			{
				internalChildCollection[i].ChildIndex = i;
			}
		}

		#endregion

		#endregion

		#region Panel Event Handler Overrides

		/// <summary>
		/// Overrides the standard OnVisualChildrenChanged method from the <see cref="System.Windows.Controls.Panel"/> class.
		/// </summary>
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
		{
			resizeInProgress = false;
			initialSizeChangeInProgress = false;

			if (visualAdded != null)
			{
				// If a child was added, switch all neccessary flags ...
				newChildAdded = true;
				addedUIElement = visualAdded as UIElement;
				removedUIElement = null;
			}
			else
			{
				// ... else a child was removed, so also switch all neccessary flags
				newChildAdded = false;
				addedUIElement = null;
				removedUIElement = visualRemoved as UIElement;
			}

			base.OnVisualChildrenChanged( visualAdded, visualRemoved );
		}

		#endregion

		#region Panel Event Handler

		//private void AnimatedSlotPanel_SizeChanged(object sender, SizeChangedEventArgs e)
		//{
		//	resizeInProgress = true;
		//	initialSizeChangeInProgress = e.PreviousSize.Equals( new Size( 0, 0 ) );
		//}

		#endregion
	}
}
