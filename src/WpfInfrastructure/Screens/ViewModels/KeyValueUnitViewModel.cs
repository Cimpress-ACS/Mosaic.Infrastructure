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
using System.Linq;
using Caliburn.Micro;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    /// <summary>
    /// Exposes the values of a tag particularly the name, the value and the unit.
    /// </summary>
    public class KeyValueUnitViewModel : PropertyChangedBase
    {
        private readonly MasterDetailViewModel _parentViewModel;
        private readonly BindableCollection<KeyValueUnitViewModel> _children;
        private Tag _tag;
        private KeyValueChange _pendingChange;

        /// <summary>
        /// Intitializes a new <see cref="KeyValueUnitViewModel"/>.
        /// </summary>
        /// <remarks>
        /// Creates a dummy <see cref="Tag"/> instance as model.
        /// Does not assign a <see cref="MasterDetailViewModel"/> as parent, though.
        /// </remarks>
        public KeyValueUnitViewModel()
        {
            _tag = new Tag();
        }

        /// <summary>
        /// Initializes a new <see cref="KeyValueUnitViewModel"/>.
        /// </summary>
        /// <param name="parentViewModel">The parent view model</param>
        /// <param name="tag">The tag providing the data.</param>
        public KeyValueUnitViewModel(MasterDetailViewModel parentViewModel, Tag tag)
            : this()
        {
            _parentViewModel = parentViewModel;
            _children = new BindableCollection<KeyValueUnitViewModel>(tag.Children.Select(t => new KeyValueUnitViewModel(parentViewModel, t)));
            _tag = tag;
            _pendingChange = null;
        }

        /// <summary>
        /// The child tags to display on the ui.
        /// </summary>
        public BindableCollection<KeyValueUnitViewModel> Children
        {
            get { return _children; }
        }

        /// <summary>
		/// The unique key as string.
		/// </summary>
		public string Key
		{
			get { return _tag.Key; }
		}

        /// <summary>
		/// The name to display in the view.
		/// </summary>
		public string Name
		{
			get { return _tag.Name; }
		}

        /// <summary>
        /// The value to display on the view.
        /// </summary>
        public object Value
		{
			get
			{
			    if (_pendingChange == null)
			    {
			        return _tag.Value;
			    }
			    return _pendingChange.NewValue;
			}
			set
			{
                if (Value == value) 
                    return;
                _pendingChange = new KeyValueChange(Key, Name, value, _tag.Value, Type);
				NotifyOfPropertyChange( () => Value );
                NotifyOfPropertyChange( () => IsDirty );
                _parentViewModel.KeyValueChanged(_pendingChange);
			}
		}

        /// <summary>
        /// The .net <see cref="Type"/> of the value.
        /// </summary>
        public Type Type
        {
            get { return _tag.Type; }
        }

        /// <summary>
        /// Determines if this value is readonly.
        /// </summary>
        public bool IsReadOnly
		{
			get { return _tag.IsReadOnly; }
		}

        /// <summary>
        /// Determines if this value was changed on the ui.
        /// </summary>
        public bool IsDirty
        {
            get { return _pendingChange != null; }
        }

        /// <summary>
        /// The unit to display on the view.
        /// </summary>
        public string Unit
		{
			get { return _tag.Unit; }
		}

        public List<EnumerationMember> EnumerationMembers
        {
            get { return _tag.EnumerationMembers; }
        }

        public EnumerationMember SelectedEnumerationMember
        {
            get { return EnumerationMembers.First(e => e.Value == (short) Value); }
            set { Value = value.Value; }
        }

        /// <summary>
        /// The comment to display on the view.
        /// </summary>
        public string Comment
		{
			get { return _tag.Comment; }
		}

        /// <summary>
        /// Updates all the tag related values exposed by this view model.
        /// </summary>
        /// <param name="newTag">The new tag which values should get exposed.</param>
        public virtual void Update(Tag newTag)
        {
            _tag = newTag;
            foreach (Tag child in newTag.Children)
            {
                KeyValueUnitViewModel childViewModel = _children.FirstOrDefault(c => string.Equals(child.Name, c.Name));
                if (childViewModel != null)
                    childViewModel.Update(child);
            }
            NotifyOfPropertyChange(string.Empty);
        }

        /// <summary>
        /// Resets all the changes made on the ui.
        /// </summary>
        public void Reset()
        {
            _pendingChange = null;
            foreach (KeyValueUnitViewModel child in _children)
                child.Reset();
            NotifyOfPropertyChange(string.Empty);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("KeyValueUnitViewModel: Name = '{0}'", Name);
        }
    }
}
