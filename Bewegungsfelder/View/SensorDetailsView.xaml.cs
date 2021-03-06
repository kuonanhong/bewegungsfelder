﻿/*
Part of Bewegungsfelder

MIT-License
(C) 2016 Ivo Herzig

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Bewegungsfelder.VM;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Bewegungsfelder.View
{
    /// <summary>
    /// Interaction logic for SensorDetailsView.xaml
    /// </summary>
    public partial class SensorDetailsView : UserControl
    {
        DispatcherTimer refreshTimer;

        private List<DataPoint> xAccelData = new List<DataPoint>();
        private List<DataPoint> yAccelData = new List<DataPoint>();
        private List<DataPoint> zAccelData = new List<DataPoint>();

        private List<DataPoint> xGyroData = new List<DataPoint>();
        private List<DataPoint> yGyroData = new List<DataPoint>();
        private List<DataPoint> zGyroData = new List<DataPoint>();

        public SensorVM Sensor
        {
            get { return (SensorVM)GetValue(SensorProperty); }
            set { SetValue(SensorProperty, value); }
        }

        public static readonly DependencyProperty SensorProperty =
            DependencyProperty.Register(nameof(Sensor), typeof(SensorVM), typeof(SensorDetailsView),
                new FrameworkPropertyMetadata(OnSensorPropertyChanged));


        public SensorDetailsView()
        {
            InitializeComponent();

            refreshTimer = new DispatcherTimer(DispatcherPriority.Background);
            refreshTimer.Interval = TimeSpan.FromMilliseconds(100);
            refreshTimer.Tick += OnRefreshTimerTick;

            xAccel.ItemsSource = xAccelData;
            yAccel.ItemsSource = yAccelData;
            zAccel.ItemsSource = zAccelData;

            xGyro.ItemsSource = xGyroData;
            yGyro.ItemsSource = yGyroData;
            zGyro.ItemsSource = zGyroData;
        }

        private void OnRefreshTimerTick(object sender, EventArgs e)
        {
            xAccelData.Add(new DataPoint(Sensor.LastValue.SensorTimestamp, Sensor.LastValue.Acceleration.X));
            yAccelData.Add(new DataPoint(Sensor.LastValue.SensorTimestamp, Sensor.LastValue.Acceleration.Y));
            zAccelData.Add(new DataPoint(Sensor.LastValue.SensorTimestamp, Sensor.LastValue.Acceleration.Z));

            xGyroData.Add(new DataPoint(Sensor.LastValue.SensorTimestamp, Sensor.LastValue.Gyro.X));
            yGyroData.Add(new DataPoint(Sensor.LastValue.SensorTimestamp, Sensor.LastValue.Gyro.Y));
            zGyroData.Add(new DataPoint(Sensor.LastValue.SensorTimestamp, Sensor.LastValue.Gyro.Z));

            int numSamples = 100;
            if (xAccelData.Count > numSamples)
                xAccelData.RemoveRange(0, xAccelData.Count - numSamples);
            if (yAccelData.Count > numSamples)
                yAccelData.RemoveRange(0, yAccelData.Count - numSamples);
            if (zAccelData.Count > numSamples)
                zAccelData.RemoveRange(0, zAccelData.Count - numSamples);

            if (xGyroData.Count > numSamples)
                xGyroData.RemoveRange(0, xGyroData.Count - numSamples);
            if (yGyroData.Count > numSamples)
                yGyroData.RemoveRange(0, yGyroData.Count - numSamples);
            if (zGyroData.Count > numSamples)
                zGyroData.RemoveRange(0, zGyroData.Count - numSamples);

            plot_accel.InvalidatePlot();
            plot_gyro.InvalidatePlot();
        }

        private static void OnSensorPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var view = (SensorDetailsView)obj;
            view.xAccelData.Clear();
            view.yAccelData.Clear();
            view.zAccelData.Clear();
            view.plot_accel.InvalidatePlot();
            view.plot_gyro.InvalidatePlot();

            if (e.NewValue == null)
                view.refreshTimer.Stop();
            else
                view.refreshTimer.Start();

            view.DataContext = e.NewValue;
        }
    }
}
