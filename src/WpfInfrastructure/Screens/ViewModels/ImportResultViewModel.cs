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
using Caliburn.Micro;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    public class ImportExportResultViewModel : PropertyChangedBase
    {
        private readonly ICollection<string> _failures;
        private bool _isVisible;
        private bool _hasFailed;

        public ImportExportResultViewModel()
        {
            _isVisible = true;
            _failures = new List<string>();
        }

        public virtual bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyOfPropertyChange(() => IsVisible);
                }
            }
        }

        public bool HasFailed
        {
            get { return _hasFailed; }
            set
            {
                if (_hasFailed != value)
                {
                    _hasFailed = value;
                    NotifyOfPropertyChange(() => HasFailed);
                }
            }
        }

        public string Failures
        {
            get { return string.Join(Environment.NewLine+Environment.NewLine, _failures); }
        }

        public void Close()
        {
            IsVisible = false;
        }

        public void SetFailures(IEnumerable<string> failures)
        {
            _failures.Clear();
            if (failures == null)
                return;
            foreach (string failure in failures)
                _failures.Add(failure);
        }

        public override string ToString()
        {
            return string.Format("{0} < HasFailed = '{1}', Failures = '{2}' >", GetType().Name, _hasFailed,
                string.Join(Environment.NewLine, _failures));
        }
    }

    public class ImportExportResultNullObject : ImportExportResultViewModel
    {
        public override bool IsVisible
        {
            get { return false; }
            set { }
        }
    }
}
