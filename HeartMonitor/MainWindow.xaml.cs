using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using WSMonitor;

namespace HeartMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsOpen = true;
        public long Start;
        private List<double> y = new List<double>();
        private List<Reading> _readings = new List<Reading>();
        private int _height;
        private double _scale;
        public MainWindow()
        {
            InitializeComponent();
            _height = (int)this.Height;
            _scale = 1024 / _height;
            _scale *= 1.2;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(WatchData) { IsBackground = true }.Start();
            new Thread(AnalyzeBpm).Start();
        }

        private void WatchData()
        {
            Start = DateTime.Now.Millisecond;
            while (IsOpen)
            {
                var ip = Discovery.FindMonitor();
                var monitor = new WSMonitor.Monitor(ip);
                monitor.ValueReceived += Monitor_ValueReceived;
                monitor.Error += Monitor_Error;
                monitor.Read();
                monitor.ValueReceived -= Monitor_ValueReceived;
                monitor.Error -= Monitor_Error;
            }
        }

        private int _avg = 0;
        private void AnalyzeBpm()
        {
            while (IsOpen)
            {
                try
                {
                    if (_readings.Count > 200)
                    {
                        var readings = _readings.ToList();
                        readings = readings.Skip(readings.Count - 200).ToList();
                        var detectedBeats = new List<Reading>();
                        for (var i = 0; i < readings.Count - 4; i++)
                        {
                            if (readings.ToArray()[i + 1].Value < readings.ToArray()[i].Value)
                            {
                                var cur = readings.ElementAt(i).Value;
                                var c1 = readings.ElementAt(i + 1).Value;
                                var c2 = readings.ElementAt(i + 2).Value;
                                var c3 = readings.ElementAt(i + 3).Value;

                                if (cur - c1 > 600 || cur - c2 > 600 || cur - c3 > 600)
                                {
                                    detectedBeats.Add(readings.ElementAt(i));
                                    i += 3;
                                }
                            }
                        }

                        //get average time between the beats
                        if (detectedBeats.Count < 2)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                        var times = detectedBeats.OrderBy(x => x.Milliseconds).Select(x => x.Milliseconds).ToArray();
                        var lengths = new List<long>();
                        for (var i = 1; i < times.Length; i++)
                        {
                            lengths.Add(times[i] - times[i - 1]);
                        }
                        var avgTime = lengths.Average();
                        var bpm = (int)(60000 / avgTime);
                        Dispatcher.Invoke(new Action(() => BPM.Content = bpm));
                    }
                }
                catch (Exception e)
                {
                    //Debugger.Break();
                }
                Thread.Sleep(1000);
            }
        }

        private void Monitor_Error(object sender, string e)
        {
            //Console.WriteLine(e);
        }

        private void Monitor_ValueReceived(object sender, Reading e)
        {
            Dispatcher.Invoke(new Action(() => UpdateValue(e)));
            _readings.Add(e);
            if (_readings.Count > 300)
                _readings = _readings.Skip(_readings.Count - 300).ToList();
        }

        

        private void UpdateValue(Reading e)
        {
            //var raw = e.Value;
            //var k = 0.5;
            //var l = 1.5;
            //_filteredValue = k * raw + (1 - k) * _filteredValue + l * (raw - _filteredValue);

            y.Add(((1024 - (double)e.Value) / _scale) + 20);

            if (y.Count > 400)
            {
                y.RemoveAt(0);
            }
            //build our data path
            var data = "M";
            for (var i = 0; i < y.Count; i++)
            {
                data += $" {i},{(int)y[i]}";
            }

            ECGPath.Data = Geometry.Parse(data);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            IsOpen = false;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //_height = (int)this.Height;
            //_scale = 1024 / _height;
            //_scale *= 1.2;
        }
    }
}
