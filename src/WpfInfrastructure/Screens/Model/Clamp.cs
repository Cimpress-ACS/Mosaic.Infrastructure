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

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Model
{
    public class Clamp : PropertyChangedBase
    {
        private long _id;
        private DateTime _timeStamp;
        private Shirt _currentShirt;
        private bool _isEmpty;
        private bool _isRemoving;
        private bool _isPOI;

        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set
            {
                if (value != _timeStamp)
                {
                    _timeStamp = value;
                    NotifyOfPropertyChange(() => TimeStamp);
                }
            }
        }

        public Shirt CurrentShirt
        {
            get { return _currentShirt; }
            set
            {
                if (value != _currentShirt)
                {
                    _currentShirt = value;
                    NotifyOfPropertyChange(() => CurrentShirt);
                }
            }
        }

        public bool IsEmpty
        {
            get { return _isEmpty; }
            set
            {
                if (value != _isEmpty)
                {
                    _isEmpty = value;
                    NotifyOfPropertyChange(() => IsEmpty);
                }
            }
        }

        public bool IsRemoving
        {
            get { return _isRemoving; }
            set
            {
                if (value != _isRemoving)
                {
                    _isRemoving = value;
                    NotifyOfPropertyChange(() => IsRemoving);
                }
            }
        }

        public bool IsPOI
        {
            get { return _isPOI; }
            set
            {
                if (value != _isPOI)
                {
                    _isPOI = value;
                    NotifyOfPropertyChange(() => IsPOI);
                }
            }
        }

        public bool Equals(Clamp other)
        {
            if (other == null)
                return false;

            return ID == other.ID;
        }

        public override bool Equals(object other)
        {
            return Equals(other as Clamp);
        }

        public long ID
        {
            get { return _id; }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyOfPropertyChange(() => ID);
                }
            }
        }

        public override int GetHashCode()
        {
            return (int)ID;
        }

        public override string ToString()
        {
            return string.Format("Clamp < ID = '{0}' >", _id);
        }
    }
}
