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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Shapes;

namespace VP.FF.PT.Common.WpfInfrastructure.Controls
{
    public class TemperatureChart : Chart
    {
        private LinearAxis Time { get; set; }
        private List<ObservableCollection<KeyValuePair<int, double>>> valueLists;
        private int DisplayRange { get; set; }

        public TemperatureChart(string graphName, int width, int height, string linelegend, string caption, int temperatureMinimum, int temperatureMaximum, int temperatureInterval, int timeRange, int displayedTimeInterval, bool showGridLines, double startTemperature = 0)
        {
            this.Height = height;
            this.Width = width;
            this.Title = graphName;
            DisplayRange = displayedTimeInterval;
            valueLists = new List<ObservableCollection<KeyValuePair<int, double>>>();

            Time = new LinearAxis();
            Time.Orientation = AxisOrientation.X;
            Time.Title = "Time(minutes)";
            Time.Minimum = 0;
            Time.Interval = timeRange;
            Time.ShowGridLines = showGridLines;
            this.Axes.Add(Time);

            ObservableCollection<KeyValuePair<int, double>> valueList = new ObservableCollection<KeyValuePair<int, double>>();
            valueList.Add(new KeyValuePair<int, double>(0, startTemperature));
            valueLists.Add(valueList);

            LineSeries firstLineSerie = new LineSeries();
            firstLineSerie.Title = linelegend;
            firstLineSerie.DependentValuePath = "Value";
            firstLineSerie.IndependentValuePath = "Key";
            firstLineSerie.IsSelectionEnabled = true;
            firstLineSerie.ItemsSource = valueLists.First();
            LinearAxis ax = new LinearAxis();
            ax.Orientation = AxisOrientation.Y;
            ax.Title = caption;
            ax.Minimum = temperatureMinimum;
            ax.Maximum = temperatureMaximum;
            ax.Interval = temperatureInterval;
            firstLineSerie.DependentRangeAxis = ax;
            this.Series.Add(firstLineSerie);

        }

        public void AddValue(double value, int line = 0)
        {

            int lastTime = valueLists[line].Last().Key;
            lastTime++;
            valueLists[line].Add(new KeyValuePair<int, double>(lastTime, value));
            UpdateTimeScale();
            PreventMemoryLeaks(lastTime, line);
        }

        internal void UpdateTimeScale()
        {
            int tempval = valueLists.First().Last().Key;
            if (tempval > DisplayRange)
                Time.Minimum++;
        }

        internal void PreventMemoryLeaks(int counter, int line)
        {
            if (counter > 100)
                DeleteValues(line);
        }

        internal void DeleteValues(int line)
        {
            valueLists[line].RemoveAt(0);
        }

        public void GridLinesToggle()
        {
            if (Time.ShowGridLines == false)
                Time.ShowGridLines = true;
            else
                Time.ShowGridLines = false;
        }

        //for future reference, in case of need
        /*public void AddLine(string linelegend,string caption,int startTemperature)
        {
            ObservableCollection<KeyValuePair<int, double>> valueList = new ObservableCollection<KeyValuePair<int, double>>();
            valueList.Add(new KeyValuePair<int, double>(0, startTemperature));
            valueLists.Add(valueList);
            LineSeries newLine = new LineSeries();
            newLine.Title = linelegend;
            newLine.DependentValuePath = "Value";
            newLine.IndependentValuePath = "Key";
            newLine.IsSelectionEnabled = true;
            newLine.ItemsSource = valueLists[1];
            newLine.DependentRangeAxis = (LinearAxis)((LineSeries)this.Series[0]).DependentRangeAxis;
            this.Series.Add(newLine);

        }*/


    }
}
