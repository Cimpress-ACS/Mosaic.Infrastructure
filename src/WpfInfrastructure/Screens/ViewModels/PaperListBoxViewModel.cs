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
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Threading;
using Caliburn.Micro;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.WpfInfrastructure.Extensions;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Views;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels
{
    public enum PaperChangeType
    {
        PaperDetected,
        PaperRemoved,
    }

    public class PaperListBoxViewModel : Screen
    {
        private readonly ILogger _logger;

        private Foil _currentFoil;
        private BindableCollection<Foil> _foils;
        private bool _isEnabled;
        private int _rows;
        private int _columns;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaperListBoxViewModel"/> class.
        /// </summary>
        public PaperListBoxViewModel()
        {
            _logger = new Log4NetLogger();
            _logger.Init(GetType());
        }

        public int Rows
        {
            get { return _rows; }
            set
            {
                if (value != _rows)
                {
                    _rows = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public int Columns
        {
            get { return _columns; }
            set
            {
                if (value != _columns)
                {
                    _columns = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public bool IsEnabled
        {
            get { return _isEnabled; }

            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    NotifyOfPropertyChange(() => IsEnabled);
                }
            }
        }

        public Foil CurrentFoil
        {
            get { return _currentFoil; }
            set
            {
                if (_currentFoil != value)
                {
                    _currentFoil = value;
                    NotifyOfPropertyChange(() => CurrentFoil);
                }
            }
        }

        public BindableCollection<Foil> Foils
        {
            get { return _foils; }
            set
            {
                if (value != _foils)
                {
                    _foils = value;
                    NotifyOfPropertyChange(() => Foils);
                }
            }
        }

        public Foil CurrentItem
        {
            get { return _currentFoil; }
            set
            {
                if (value != _currentFoil)
                {
                    _currentFoil = value;
                    NotifyOfPropertyChange(() => CurrentItem);
                }
            }
        }

        private void ExecuteDeleteSpecificPaper(Foil existingFoil)
        {
            _logger.Debug(string.Format("Execute delete of specific paper (paper='{0}'", existingFoil));

            const double deletionDelay = 700;

            var deletionTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(deletionDelay) };
            deletionTimer.Tick += (o, args) =>
            {
                _logger.Debug(string.Format("Remove clamp (clamp='{0}'", existingFoil));
                Foils.Remove(existingFoil);
                deletionTimer.Stop();
                EventHelper.RemoveAllEventHandler(deletionTimer, "Tick");
            };

            deletionTimer.Start();
            existingFoil.IsRemoving = true;
        }

        /// <summary>
        /// Used to fadeout the view before resetting the current item that collapses the view.
        /// </summary>
        public void StartClose()
        {
            var timer = new Timer { Interval = 300, AutoReset = false };
            timer.Elapsed += (sender, evt) =>
            {
                Close();
                timer.Dispose();
                timer = null;
            };
            timer.Start();
        }

        public void Close()
        {
            CurrentItem = null;
        }

    }
}
