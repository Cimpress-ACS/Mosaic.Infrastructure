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
using Caliburn.Micro;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    /// <summary>
    /// Exposes the values of a tag particularly the name, the value and the unit.
    /// </summary>
    public class ForcingKeyValueUnitViewModel : PropertyChangedBase
    {
        private bool _isValueEditable;
        private object _value;
        private object _realValue;
        private object _forceValue;
        private readonly ForcingTag _tag;

        public ForcingKeyValueUnitViewModel(ForcingTag tag)
        {
            _tag = tag;
        }

        //TODO: Rolf, please check this
        public bool IsValueEditable
        {
            get { return _isValueEditable; }

            set
            {
                if (value != _isValueEditable)
                {
                    _isValueEditable = value;
                    NotifyOfPropertyChange(() => IsValueEditable);
                }
            }
        }

        public object Value
        {
            get { return _value; }

            set
            {
                if (value != _value)
                {
                    _value = value;
                    NotifyOfPropertyChange(() => Value);
                }
            }
        }

        public object RealValue
        {
            get { return _realValue; }

            set
            {
                if (value != _realValue)
                {
                    _realValue = value;
                    NotifyOfPropertyChange(() => RealValue);
                }
            }
        }

        public object ForceValue
        {
            get { return _forceValue; }

            set
            {
                if (value != _forceValue)
                {
                    _forceValue = value;
                    NotifyOfPropertyChange(() => ForceValue);
                }
            }
        }

        //TODO: Rolf, please check this
        public bool IsEnabled
        {
            get { return _tag.IsEnabled; }

            set
            {
                if (value != _tag.IsEnabled)
                {
                    _tag.IsEnabled = value;
                    NotifyOfPropertyChange(() => IsEnabled);
                }
            }
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
        /// The .net <see cref="Type"/> of the value.
        /// </summary>
        public Type Type
        {
            get { return _tag.Type; }
        }

        /// <summary>
        /// The unit to display on the view.
        /// </summary>
        public string Unit
        {
            get { return _tag.Unit; }
        }

        /// <summary>
        /// The comment to display on the view.
        /// </summary>
        public string Comment
        {
            get { return _tag.Comment; }
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
            return string.Format("ForcingKeyValueUnitViewModel: Name = '{0}'", Name);
        }
    }
}
