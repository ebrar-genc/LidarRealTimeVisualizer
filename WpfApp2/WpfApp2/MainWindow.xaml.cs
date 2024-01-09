using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            LidarDataSubscriber subscriber;

            InitializeComponent();
            subscriber = new LidarDataSubscriber("tcp://localhost:3001");
            subscriber.ListenForMessages();
        }

       /* public void DrawLidarData(double[] angles, double[] distances)
        {
            lidarCanvas.Children.Clear();

            for (int i = 0; i < angles.Length; i++)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = 5,
                    Height = 5,
                    Fill = Brushes.Blue
                };

                double x = distances[i] * Math.Cos(angles[i]);
                double y = distances[i] * Math.Sin(angles[i]);

                Canvas.SetLeft(ellipse, x);
                Canvas.SetTop(ellipse, y);

                lidarCanvas.Children.Add(ellipse);
            }
        }*/
    }

}

