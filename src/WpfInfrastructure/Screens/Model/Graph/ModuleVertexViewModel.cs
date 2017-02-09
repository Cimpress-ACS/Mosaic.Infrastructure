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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Media.Imaging;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using Point = System.Windows.Point;

// ReSharper disable CheckNamespace
namespace QuickGraph.Objects
// ReSharper restore CheckNamespace
{
    [DebuggerDisplay("{ID} - {Counter}/{Capacity}, State: {State}")]
    public class ModuleVertexViewModel : ModuleVertexViewModelBase
    {
        private const int Margin = 30;
        private const int FontHeight = 20;
        private string _displayName;
        private BitmapImage _image;
        private ModuleState _state;
        private bool _hasWarnings;
        private bool _hasErrors;
        private string _alertText;
        private bool _showBorder;
        private int _capacity;
        private bool _isAdornerVisible;
        private double _adornerOpacity;
        private int _counter;
        private bool _isBusy;
        private bool _isManualControllerMode;
        private bool _isSimulationControllerMode;


        public ModuleVertexViewModel()
        {
            DefaultVertexHeight = 700;
        }

        public ModuleVertexViewModel(string id, string imagepath, Point pos, bool hasErrors = false)
        {
            ID = id;
            if (!string.IsNullOrEmpty(imagepath) && File.Exists(imagepath))
            {
                Image = new BitmapImage(new Uri(imagepath, UriKind.Relative));
            }
            else
            {
                ShowBorder = true;
            }

            Position = pos;
            DefaultVertexHeight = 700;

            HasWarnings = _hasWarnings;
            HasErrors = hasErrors;

            IsAdornerVisible = true;
            AdornerOpacity = 1;
        }

        public BitmapImage Image
        {
            get { return _image; }
            set
            {
                _image = value;

                NotifyOfPropertyChange(() => Image);
                NotifyOfPropertyChange(() => ImageHeight);
                NotifyOfPropertyChange(() => VertexHeight);
            }
        }

        public double ImageHeight
        {
            get
            {
                if (Image != null)
                {
                    return Image.PixelHeight;
                }
                return 0;
            }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (value != _displayName)
                {
                    _displayName = value;
                    NotifyOfPropertyChange(() => DisplayName);
                }
            }
        }

        public double DefaultVertexHeight { get; set; }

        public double VertexHeight
        {
            get
            {
                if (ShowBorder)
                    return DefaultVertexHeight;

                return ImageHeight + Margin + FontHeight;
            }
        }

        public ModuleState State
        {
            get { return _state; }
            set
            {
                if (_state == value) return;
                    _state = value;
                NotifyOfPropertyChange(() => State);
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    NotifyOfPropertyChange(() => IsBusy);
                }
            }
        }

        public bool IsManualControllerMode
        {
            get { return _isManualControllerMode; }
            set
            {
                if (value != _isManualControllerMode)
                {
                    _isManualControllerMode = value;
                    NotifyOfPropertyChange(() => IsManualControllerMode);
                }
            }
        }

        public bool IsSimulationControllerMode
        {
            get { return _isSimulationControllerMode; }
            set
            {
                if (value != _isSimulationControllerMode)
                {
                    _isSimulationControllerMode = value;
                    NotifyOfPropertyChange(() => IsSimulationControllerMode);
                }
            }
        }

        public bool HasWarnings
        {
            get { return _hasWarnings; }
            set
            {
                if (_hasWarnings == value) return;
                _hasWarnings = value;
                NotifyOfPropertyChange(() => HasWarnings);
                NotifyOfPropertyChange(() => AlertText);
            }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
            set
            {
                if (_hasErrors == value) return;
                _hasErrors = value;
                NotifyOfPropertyChange(() => HasErrors);
                NotifyOfPropertyChange(() => AlertText);
            }
        }

        public string AlertText
        {
            get { return _alertText; }
            set
            {
                if (_alertText == value) return;
                _alertText = value;
                NotifyOfPropertyChange(() => AlertText);
            }
        }

        public bool ShowBorder
        {
            get { return _showBorder; }
            set
            {
                if (_showBorder == value) return;
                _showBorder = value;
                NotifyOfPropertyChange(() => ShowBorder);
            }
        }

        public int Counter
        {
            get { return _counter; }
            set
            {
                if (_counter == value) return;
                _counter = value;
                NotifyOfPropertyChange(() => Counter);
                NotifyOfPropertyChange(() => CounterLabel);
            }
        }

        public int Capacity
        {
            get { return _capacity; }
            set
            {
                if (_capacity == value) return;
                _capacity = value;
                NotifyOfPropertyChange(() => Capacity);
                NotifyOfPropertyChange(() => CounterLabel);
            }
        }

        public string CounterLabel
        {
            get
            {
                var counterDisplay = Counter > 0 ? Counter.ToString(CultureInfo.InvariantCulture) : string.Empty;
                var capacityDisplay = Capacity > 0 && Counter > 0 ? " / " + Capacity : string.Empty;
                var label = string.Format("{0}{1}", counterDisplay, capacityDisplay);
                return label;
            }
        }

        public bool IsAdornerVisible
        {
            get { return _isAdornerVisible; }
            set
            {
                if (_isAdornerVisible == value) return;
                _isAdornerVisible = value;
                NotifyOfPropertyChange(() => IsAdornerVisible);
            }
        }

        public double AdornerOpacity
        {
            get { return _adornerOpacity; }
            set
            {
                if (_adornerOpacity.Equals(value)) return;
                _adornerOpacity = value;
                NotifyOfPropertyChange(() => IsAdornerVisible);
            }
        }
    }
}
