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
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Color = System.Drawing.Color;

namespace VP.FF.PT.Common.WpfInfrastructure.Screens.Model
{
    using System.Collections.ObjectModel;
    using System.IO;

    using Extensions;

    /// <summary>
    /// Shirt view model contains shirt color, size, state and preview images.
    /// </summary>
    public class Shirt : PropertyChangedBase
    {
        private BitmapImage _frontPrint;
        private BitmapImage _backPrint;
        private ShirtState _shirtState;
        private string _flaggedComment;
        private Uri _shirtTypeImage;
        private Uri _shirtTypeImageOutline;

        private string _barcode;

        private double _size = 1.0;
        private Color _color;
        private string _shirtType;
        private string _shirtMaterialType;
        private OriginLabel _originLabel;

        private ObservableCollection<string> _route;
        private int _selectedRouteIndex;

        private DateTime _lastRfidRead;
        private long _rfidReadCounter;
        private TimeSpan _timeInSystem;

        private bool _isFrontFoilPending;
        private bool _isBackFoilPending;

        private string _jobId;
        private string _itemId;
        private string _skuId;
        private string _logHistory;

        public void NotifyAll()
        {
            NotifyOfPropertyChange(() => ShirtType);
            NotifyOfPropertyChange(() => ShirtMaterialType);
            NotifyOfPropertyChange(() => ShirtState);
            NotifyOfPropertyChange(() => IsFlagged);
            NotifyOfPropertyChange(() => Color);
            NotifyOfPropertyChange(() => Size);
            NotifyOfPropertyChange(() => FrontPrint);
            NotifyOfPropertyChange(() => BackPrint);
            NotifyOfPropertyChange(() => ShirtTypeImage);
            NotifyOfPropertyChange(() => ShirtTypeImageOutline);
            NotifyOfPropertyChange(() => Barcode);
            NotifyOfPropertyChange(() => Route);
            NotifyOfPropertyChange(() => SelectedRouteIndex);
            NotifyOfPropertyChange(() => LastRfidRead);
            NotifyOfPropertyChange(() => RfidReadCounter);
            NotifyOfPropertyChange(() => TimeInSystem);
            NotifyOfPropertyChange(() => FlaggedComment);
            NotifyOfPropertyChange(() => IsFrontFoilPending);
            NotifyOfPropertyChange(() => IsBackFoilPending);

        }

        public string SkuId
        {
            get { return _skuId; }

            set
            {
                if (value != _skuId)
                {
                    _skuId = value;
                    NotifyOfPropertyChange(() => SkuId);
                }
            }
        }

        public string ShirtType
        {
            get { return _shirtType; }

            set
            {
                if (value != _shirtType)
                {
                    _shirtType = value;
                    NotifyOfPropertyChange(() => ShirtType);
                }
            }
        }

        public string ShirtMaterialType
        {
            get { return _shirtMaterialType; }

            set
            {
                if (value != _shirtMaterialType)
                {
                    _shirtMaterialType = value;
                    NotifyOfPropertyChange(() => ShirtMaterialType);
                }
            }
        }

        public ShirtState ShirtState
        {
            get { return _shirtState; }

            set
            {
                if (value != _shirtState)
                {
                    _shirtState = value;
                    NotifyOfPropertyChange(() => ShirtState);
                    NotifyOfPropertyChange(() => IsFlagged);
                }
            }
        }

        public bool IsFlagged
        {
            get
            {
                return ShirtState == ShirtState.Flagged;
            }
        }

        /// <summary>
        /// Gets or sets the color as hex color code, so that a binding in WPF is easily possible.
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color Color
        {
            get { return _color; }

            set
            {
                if (value != _color)
                {
                    _color = value;
                    NotifyOfPropertyChange(() => Color);
                }
            }
        }

        /// <summary>
        /// Gets or sets the size as a scaling factor. 
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        public double Size
        {
            get { return _size; }

            set
            {
                if (Math.Abs(value - _size) > 0.1)
                {
                    _size = value;
                    NotifyOfPropertyChange(() => Size);
                }
            }
        }

        public OriginLabel OriginLabel
        {
            get { return _originLabel; }

            set
            {
                if (value != _originLabel)
                {
                    _originLabel = value;
                    NotifyOfPropertyChange(() => OriginLabel);
                }
            }
        }

        public BitmapImage FrontPrint
        {
            get { return _frontPrint; }

            set
            {
                if (value != _frontPrint)
                {
                    _frontPrint = value;
                    NotifyOfPropertyChange(() => FrontPrint);
                }
            }
        }

        public BitmapImage BackPrint
        {
            get { return _backPrint; }

            set
            {
                if (value != _backPrint)
                {
                    _backPrint = value;
                    NotifyOfPropertyChange(() => BackPrint);
                }
            }
        }

        /// <summary>
        /// Gets or sets the bitmap representing the shirt type.
        /// </summary>
        /// <value>
        /// The bitmap representing the shirt type.
        /// </value>
        public Uri ShirtTypeImage
        {
            get { return _shirtTypeImage; }
            set
            {
                if (value != _shirtTypeImage)
                {
                    _shirtTypeImage = value;
                    NotifyOfPropertyChange(() => ShirtTypeImage);
                }
            }
        }

        /// <summary>
        /// Gets or sets the bitmap representing the outline of the shirt type.
        /// </summary>
        /// <value>
        /// The bitmap representing the outline of the shirt type.
        /// </value>
        public Uri ShirtTypeImageOutline
        {
            get { return _shirtTypeImageOutline; }
            set
            {
                if (value != _shirtTypeImageOutline)
                {
                    _shirtTypeImageOutline = value;
                    NotifyOfPropertyChange(() => ShirtTypeImageOutline);
                }
            }
        }

        public string JobId
        {
            get { return _jobId; }

            set
            {
                if (value != _jobId)
                {
                    _jobId = value;
                    NotifyOfPropertyChange(() => JobId);
                }
            }
        }

        public string ItemId
        {
            get { return _itemId; }

            set
            {
                if (value != _itemId)
                {
                    _itemId = value;
                    NotifyOfPropertyChange(() => ItemId);
                }
            }
        }

        public string Barcode
        {
            get { return _barcode; }

            set
            {
                if (_barcode != value)
                {
                    _barcode = value;
                    NotifyOfPropertyChange(() => Barcode);
                }
            }
        }

        public ObservableCollection<string> Route
        {
            get { return _route; }

            set
            {
                if (_route != value)
                {
                    _route = value;
                    NotifyOfPropertyChange(() => Route);
                }
            }
        }

        public int SelectedRouteIndex
        {
            get { return _selectedRouteIndex; }

            set
            {
                if (_selectedRouteIndex != value)
                {
                    if (value < 0 || _route.Count <= value) // this index cant be selected
                        throw new ArgumentException("SelectedRouteIndex out of Route range.");

                    _selectedRouteIndex = value;
                    NotifyOfPropertyChange(() => SelectedRouteIndex);
                }
            }
        }

        public DateTime LastRfidRead
        {
            // 24.02.14, 16:23:01
            // var dateTimeString = String.Format(_lastRfidRead.ToString("dd.MM.yy, HH:mm:ss"));

            get { return _lastRfidRead; }

            set
            {
                if (_lastRfidRead != value)
                {
                    _lastRfidRead = value;
                    NotifyOfPropertyChange(() => LastRfidRead);
                }
            }
        }

        public long RfidReadCounter
        {
            get { return _rfidReadCounter; }

            set
            {
                if (_rfidReadCounter != value)
                {
                    _rfidReadCounter = value;
                    NotifyOfPropertyChange(() => RfidReadCounter);
                }
            }
        }

        public TimeSpan TimeInSystem
        {
            // 1 Tag, 2 Std., 34 Min., 17 Sek.
            // var timeSpanString = String.Format("{0:%d} Tag, {0:%h} Std., {0:%m} Min., {0:%s} Sek.", _timeInSystem);

            get { return _timeInSystem; }

            set
            {
                if (_timeInSystem != value)
                {
                    _timeInSystem = value;
                    NotifyOfPropertyChange(() => TimeInSystem);
                }
            }
        }

        public string FlaggedComment
        {
            get { return _flaggedComment; }

            set
            {
                if (_flaggedComment != value)
                {
                    if (ShirtState != ShirtState.Flagged)
                    {
                        throw new ArgumentException("FlaggedComment can only be set on flagged shirts.");
                    }

                    _flaggedComment = value;
                    NotifyOfPropertyChange(() => FlaggedComment);
                }
            }
        }

        public bool IsFrontFoilPending
        {
            get { return this._isFrontFoilPending; }

            set
            {
                if (this._isFrontFoilPending != value)
                {
                    this._isFrontFoilPending = value;
                    this.NotifyOfPropertyChange(() => this.IsFrontFoilPending);
                }
            }
        }

        public bool IsBackFoilPending
        {
            get { return this._isBackFoilPending; }

            set
            {
                if (this._isBackFoilPending != value)
                {
                    this._isBackFoilPending = value;
                    this.NotifyOfPropertyChange(() => this.IsBackFoilPending);
                }
            }
        }

        public string LogHistory
        {
            get { return _logHistory; }

            set
            {
                if (value != _logHistory)
                {
                    _logHistory = value;
                    NotifyOfPropertyChange(() => LogHistory);
                }
            }
        }

        public Shirt()
        {
            if (DesignTimeHelper.IsInDesignModeStatic)
            {
                this.CreateRandomShirt();
                this.ShirtState = ShirtState.Flagged;
            }
        }

        private Shirt(bool isRandom)
        {
            if (isRandom)
            {
                CreateRandomShirt();
            }
        }

        private static readonly Random _random = new Random();

        public static Shirt CreateDefaultShirt(Color color)
        {
            const string shirtType = "TShirt";
            Uri shirtTypeImage;
            Uri shirtTypeImageOutline = null;

            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            if (directoryInfo != null)
            {
                string uriStringShirt = directoryInfo.FullName + "/Resources/shirt_" + shirtType + ".png";
                string uriStringOutline = directoryInfo.FullName + "/Resources/shirt_" + shirtType + "_line.png";
                if (File.Exists(uriStringShirt))
                {
                    shirtTypeImage = new Uri(uriStringShirt);
                }
                else
                {
                    shirtTypeImage =
                        new Uri(
                            "pack://application:,,,/VP.FF.PT.Common.WpfInfrastructure;Component/Styles/Resources/Images/shirt_missing_image.png",
                            UriKind.Absolute);
                }

                if (File.Exists(uriStringOutline))
                {
                    shirtTypeImageOutline = new Uri(uriStringOutline);
                }
            }
            else
            {
                shirtTypeImage =
                 new Uri(
                     "pack://application:,,,/VP.FF.PT.Common.WpfInfrastructure;Component/Styles/Resources/Images/shirt_missing_image.png",
                     UriKind.Absolute);
            }

            var shirtViewModel = new Shirt
            {
                ShirtType = shirtType,
                ShirtMaterialType = "basic",
                Size = 0.6,
                Color = color,
                ShirtTypeImage = shirtTypeImage,
                ShirtTypeImageOutline = shirtTypeImageOutline
            };

            return shirtViewModel;
        }

        private void CreateRandomShirt()
        {
            string shirtType;
            Uri shirtTypeImage;
            Uri shirtTypeImageOutline = null;
            switch (_random.Next(6))
            {
                case 0:
                    shirtType = "YouthTShirt";
                    break;
                case 1:
                    shirtType = "LongSleeveShirt";
                    break;
                case 2:
                    //shirtType = "PremiumTShirt";
                    shirtType = "TShirt";
                    break;
                case 3:
                    shirtType = "YouthTShirt";
                    break;
                case 4:
                    shirtType = "WomensTShirt";
                    break;
                default:
                    shirtType = "TShirt";
                    break;
            }

            double shirtSize;
            switch (_random.Next(7))
            {
                case 0:
                    shirtSize = 0.5;    // S
                    break;
                case 1:
                    shirtSize = 0.8;    // XXL
                    break;
                default:
                    shirtSize = 0.6;
                    break;
            }

            Color color;
            switch (_random.Next(5))
            {
                case 0:
                    color = Color.DimGray;
                    break;
                case 1:
                    color = Color.DarkOrange;
                    break;
                default:
                    color = Color.LightGray;
                    break;
            }

            ShirtState shirtState;
            switch (_random.Next(5))
            {
                case 0:
                    shirtState = ShirtState.Empty;
                    break;
                case 1:
                    shirtState = ShirtState.Flagged;
                    break;
                default:
                    shirtState = ShirtState.Loaded;
                    break;
            }

            string shirtMaterialType;
            switch (_random.Next(4))
            {
                case 0:
                    shirtMaterialType = "premium";
                    break;
                default:
                    shirtMaterialType = "basic";
                    break;
            }

            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            if (directoryInfo != null)
            {
                string uriStringShirt = directoryInfo.FullName + "/Resources/shirt_" + shirtType + ".png";
                string uriStringOutline = directoryInfo.FullName + "/Resources/shirt_" + shirtType + "_line.png";
                if (File.Exists(uriStringShirt))
                {
                    shirtTypeImage = new Uri(uriStringShirt);
                }
                else
                {
                    shirtTypeImage =
                        new Uri(
                            "pack://application:,,,/VP.FF.PT.Common.WpfInfrastructure;Component/Styles/Resources/Images/shirt_missing_image.png",
                            UriKind.Absolute);
                }

                if (File.Exists(uriStringOutline))
                {
                    shirtTypeImageOutline = new Uri(uriStringOutline);
                }
            }
            else
            {
                shirtTypeImage =
                 new Uri(
                     "pack://application:,,,/VP.FF.PT.Common.WpfInfrastructure;Component/Styles/Resources/Images/shirt_missing_image.png",
                     UriKind.Absolute);
            }

            _color = color;
            _shirtState = shirtState;
            _shirtType = shirtType;
            _shirtMaterialType = shirtMaterialType;
            _size = shirtSize;
            _originLabel = new OriginLabel { CountryName = "Bangladesh" };
            _shirtTypeImage = shirtTypeImage;
            _shirtTypeImageOutline = shirtTypeImageOutline;
            _barcode = "12345-67039-24789";
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
            _lastRfidRead = new DateTime(2014, 2, 24, 16, 23, 1);
            _rfidReadCounter = 1337;
            _timeInSystem = new TimeSpan(1, 2, 34, 17);
            _flaggedComment =
                "Lorem ipsum dolor sit amet, consete ur sadip scing elitr, sed diam non u my eirmod tempor invidunt ut labore et dolore magna aliquyam erat";
            _isFrontFoilPending = _random.Next(0, 2) == 0;
            _isBackFoilPending = _random.Next(0, 2) == 0;

            if (_random.Next(0, 3) == 0)
            {
                _frontPrint = LoadImageHelper.LoadImage("dummy");
            }

            if (_random.Next(0, 3) == 0)
            {
                _backPrint = LoadImageHelper.LoadImage("dummy");
            }
            // front/back preview not needed for loading station
        }

        public static Shirt GetRandomShirt()
        {
            return new Shirt(true);
        }

    }
}
