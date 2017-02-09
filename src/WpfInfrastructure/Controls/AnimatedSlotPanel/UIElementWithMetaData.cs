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

namespace VP.FF.PT.Common.WpfInfrastructure.Controls
{
    public class UIElementWithMetaData
    {
        /// <summary>
        /// Gets or sets the observed <see cref="System.Windows.UIElement"/>.
        /// </summary>
        public UIElement Child;

        /// <summary>
        /// Gets or sets the specific Index of the observed <see cref="System.Windows.UIElement"/>.
        /// </summary>
        public int ChildIndex;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="System.Windows.UIElement"/> was recently added.
        /// </summary>
        public bool IsRecentlyAdded;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="System.Windows.UIElement"/> is marked as removed.
        /// </summary>
        public bool IsRemoved;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="System.Windows.UIElement"/> is marked for deletion.
        /// </summary>
        public bool IsMarkedForDeletion;

        /// <summary>
        /// Gets or sets the current position data of the <see cref="System.Windows.UIElement"/>.
        /// </summary>
        public Point CurrentPosition;

        /// <summary>
        /// Gets or sets a value indicating whether a horizontal animation of the <see cref="System.Windows.UIElement"/> is in progress.
        /// </summary>
        public bool HorizontalAnimationInProgress;
    }
}
