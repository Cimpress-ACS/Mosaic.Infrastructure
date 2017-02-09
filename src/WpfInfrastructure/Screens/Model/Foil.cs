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


namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Model
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Media.Imaging;

    using global::Caliburn.Micro;

    using VP.FF.PT.Common.WpfInfrastructure.Converters;
    using VP.FF.PT.Common.WpfInfrastructure.Extensions;

    /// <summary>
    /// TODO: Please comment
    /// </summary>
    public class Foil : PropertyChangedBase
    {
        private bool _isFrontSide;
        private bool _isPOI;

        private BitmapImage _previewImage;
        private QualityIssueControl _status;

        private string _barCode;

        private int _targetPrintingLine;
        private int _targetRotaryHeatPress;
        private bool _isRemoving;

        private string _batchId;
        private int _batchPosition;
        private int _batchCount;

        private ObservableCollection<string> _route;
        private int _selectedRouteIndex;

        private TimeSpan _timeInSystem;

        private string _logHistory;

        /// <summary>
        /// Gets or sets if the foil is a point of interest.
        /// </summary>
        /// <value>
        /// Whether the foil is a point of interest.
        /// </value>
        public bool IsPOI
        {
            get
            {
                return _isPOI;
            }

            set
            {
                if (value != _isPOI)
                {
                    _isPOI = value;
                    NotifyOfPropertyChange(() => IsPOI);
                }
            }
        }

        /// <summary>
        /// Gets or sets if the foil is for the front side.
        /// </summary>
        /// <value>
        /// Whether the foil is for the front side.
        /// </value>
        public bool IsFrontSide
        {
            get
            {
                return _isFrontSide;
            }

            set
            {
                if (value != _isFrontSide)
                {
                    _isFrontSide = value;
                    NotifyOfPropertyChange(() => IsFrontSide);
                }
            }
        }

        /// <summary>
        /// Gets or sets the preview image.
        /// </summary>
        /// <value>
        /// The preview image.
        /// </value>
        public BitmapImage PreviewImage
        {
            get
            {
                return _previewImage;
            }

            set
            {
                if (value != _previewImage)
                {
                    _previewImage = value;
                    NotifyOfPropertyChange(() => PreviewImage);
                }
            }
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public QualityIssueControl Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (value != _status)
                {
                    _status = value;
                    NotifyOfPropertyChange(() => Status);
                }
            }
        }

        /// <summary>
        /// Gets or sets the bar code identifier.
        /// </summary>
        /// <value>
        /// The bar code identifier.
        /// </value>
        public string BarCode
        {
            get
            {
                return _barCode;
            }

            set
            {
                if (value != _barCode)
                {
                    _barCode = value;
                    NotifyOfPropertyChange(() => BarCode);
                }
            }
        }

        /// <summary>
        /// Gets or sets the target printing line.
        /// </summary>
        /// <value>
        /// The target printing line.
        /// </value>
        public int TargetPrintingLine
        {
            get
            {
                return _targetPrintingLine;
            }

            set
            {
                if (value != _targetPrintingLine)
                {
                    _targetPrintingLine = value;
                    NotifyOfPropertyChange(() => TargetPrintingLine);
                }
            }
        }

        /// <summary>
        /// Gets or sets the target rotary heat press.
        /// </summary>
        /// <value>
        /// The target rotary heat press.
        /// </value>
        public int TargetRotaryHeatPress
        {
            get
            {
                return _targetRotaryHeatPress;
            }

            set
            {
                if (value != _targetRotaryHeatPress)
                {
                    _targetRotaryHeatPress = value;
                    NotifyOfPropertyChange(() => TargetRotaryHeatPress);
                }
            }
        }

        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        /// <value>
        /// The batch identifier.
        /// </value>
        public string BatchId
        {
            get
            {
                return _batchId;
            }

            set
            {
                if (value != _batchId)
                {
                    _batchId = value;
                    NotifyOfPropertyChange(() => BatchId);
                }
            }
        }

        /// <summary>
        /// Gets or sets the batch position.
        /// </summary>
        /// <value>
        /// The batch position.
        /// </value>
        public int BatchPosition
        {
            get
            {
                return _batchPosition;
            }

            set
            {
                if (value != _batchPosition)
                {
                    _batchPosition = value;
                    NotifyOfPropertyChange(() => BatchPosition);
                }
            }
        }

        /// <summary>
        /// Gets or sets the batch count.
        /// </summary>
        /// <value>
        /// The batch count.
        /// </value>
        public int BatchCount
        {
            get
            {
                return _batchCount;
            }

            set
            {
                if (value != _batchCount)
                {
                    _batchCount = value;
                    NotifyOfPropertyChange(() => BatchCount);
                }
            }
        }

        /// <summary>
        /// Gets or sets the route.
        /// </summary>
        /// <value>
        /// The route.
        /// </value>
        public ObservableCollection<string> Route
        {
            get
            {
                return _route;
            }

            set
            {
                if (value != _route)
                {
                    _route = value;
                    NotifyOfPropertyChange(() => Route);
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

        /// <summary>
        /// Gets or sets the index of the selected route.
        /// </summary>
        /// <value>
        /// The index of the selected route.
        /// </value>
        public int SelectedRouteIndex
        {
            get
            {
                return _selectedRouteIndex;
            }

            set
            {
                if (value != _selectedRouteIndex)
                {
                    _selectedRouteIndex = value;
                    NotifyOfPropertyChange(() => SelectedRouteIndex);
                }
            }
        }

        /// <summary>
        /// Gets or sets the time in system.
        /// </summary>
        /// <value>
        /// The time in system.
        /// </value>
        public TimeSpan TimeInSystem
        {
            get
            {
                return _timeInSystem;
            }

            set
            {
                if (value != _timeInSystem)
                {
                    _timeInSystem = value;
                    NotifyOfPropertyChange(() => TimeInSystem);
                }
            }
        }

        /// <summary>
        /// Gets or sets the history log.
        /// </summary>
        /// <value>
        /// The history log.
        /// </value>
        public string LogHistory
        {
            get
            {
                return _logHistory;
            }

            set
            {
                if (value != _logHistory)
                {
                    _logHistory = value;
                    NotifyOfPropertyChange(() => LogHistory);
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of alignment line which is needed by an operator to align the foil on the process table.
        /// </summary>
        /// <value>
        /// The alignment line index.
        /// </value>
        public int Alignment { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Foil" /> class.
        /// </summary>
        public Foil()
        {
            if (DesignTimeHelper.IsInDesignModeStatic)
            {
                this.CreateDesignTimeFoil();
            }
        }

        /// <summary>
        /// Random number generator.
        /// </summary>
        private static Random rand = new Random(DateTime.Now.Millisecond);

        /// <summary>
        /// Creates a design time foil.
        /// </summary>
        /// <returns></returns>
        private void CreateDesignTimeFoil()
        {
            var stateSwitch = rand.Next(1, 5);
            switch (stateSwitch)
            {
                case 1:
                    _status = QualityIssueControl.SomethingWrongWithShirt;
                    break;
                case 2:
                    _status = QualityIssueControl.WrongPrintBending;
                    break;
                case 3:
                    _status = QualityIssueControl.WrongPrintImage;
                    break;
                case 4:
                    _status = QualityIssueControl.WrongPrintOther;
                    break;
                case 5:
                    _status = QualityIssueControl.WrongCountryOfOrigin;
                    break;
            }

            _previewImage = LoadImageHelper.LoadImage("dummy");
            _barCode = "12345-67039-24789";

            _targetPrintingLine = 42;
            _targetRotaryHeatPress = 42;

            _batchId = "BATCHID42";
            _batchPosition = 7;
            _batchCount = 13;

            _route = new ObservableCollection<string>
                        {
                            "LOA",
                            "ICB",
                            "ROT",
                            "HPM",
                            "COM",
                            "ICB",
                            "LAB",
                            "FOL",
                            "BAG",
                            "MFG",
                            "RHB",
                            "WDWR",
                            "FPS",
                            "LOD",
                            "THC",
                            "OCB",
                            "WOW"
                        };
            _selectedRouteIndex = 10;

            _timeInSystem = new TimeSpan(1, 2, 34, 17);
            _logHistory =
                "Lorem ipsum dolor sit amet, consete ur sadip scing elitr, sed diam non u my eirmod tempor invidunt ut labore et dolore magna aliquyam erat";
        }

    }
}
