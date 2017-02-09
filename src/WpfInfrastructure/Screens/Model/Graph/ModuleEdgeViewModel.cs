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


using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

// ReSharper disable CheckNamespace
namespace QuickGraph.Objects
// ReSharper restore CheckNamespace
{
    [DebuggerDisplay("{Source.ID},{OriginPortIndex} -> {Target.ID}")]
    public class ModuleEdgeViewModel : Edge<ModuleVertexViewModelBase>, INotifyPropertyChanged
    {
        public ModuleEdgeViewModel(string id, ModuleVertexViewModelBase source, ModuleVertexViewModelBase target)
            : base(source, target)
        {
            ID = id;
            //ColorMe();
        }

        private string _id;
        public string ID
        {
            get { return _id; }
            set
            {
                _id = value;
                NotifyOfPropertyChanged("ID");
            }
        }

        public int OriginPortIndex { get; set; }

        private bool _firstEdge;
        public bool FirstEdge
        {
            get { return _firstEdge; }
            set
            {
                _firstEdge = value;
                NotifyOfPropertyChanged("FirstEdge");
            }
        }

        private bool _secondEdge;
        public bool SecondEdge
        {
            get { return _secondEdge; }
            set
            {
                _secondEdge = value;
                NotifyOfPropertyChanged("SecondEdge");
            }
        }

        private bool _isForcingEnabled;
        public bool IsForcingEnabled
        {
            get { return _isForcingEnabled; }

            set
            {
                if (value != _isForcingEnabled)
                {
                    _isForcingEnabled = value;

                    //ColorMe();

                    NotifyOfPropertyChanged("IsForcingEnabled");
                }
            }
        }

        private Thickness _edgeMargin;
        public Thickness EdgeMargin
        {
            get { return _edgeMargin; }
            set
            {
                _edgeMargin = value;
                NotifyOfPropertyChanged("EdgeMargin");
            }
        }

        private SolidColorBrush _foregroundBrush;
        public SolidColorBrush ForegroundBrush
        {
            get { return _foregroundBrush; }
            set
            {
                _foregroundBrush = value;
                NotifyOfPropertyChanged("ForegroundBrush");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyOfPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        //private void ColorMe()
        //{
        //    if (IsForcingEnabled)
        //    {
        //        Color color = Color.FromArgb(255, 253, 243, 149);
        //        ForegroundBrush = new SolidColorBrush(color);
        //        //ForegroundBrush = Brushes.OrangeRed;
        //    }
        //    else
        //        ForegroundBrush = Brushes.White;
        //}
    }
}
