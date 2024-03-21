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
        private LidarDataSubscriber subscriber;
        private DrawPointCloud draw;
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                subscriber = new LidarDataSubscriber("tcp://localhost:3001");
                draw = new DrawPointCloud(lidarCanvas);
                InitializeAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.Message);
            }


        }

        private async void InitializeAsync()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    var result = await subscriber.ListenForMessages();

                    double[] x = result.Item1;
                    double[] y = result.Item2;
                    Dispatcher.Invoke(() =>
                    {
                        draw.DrawPoints(x, y);

                    });
                }
            });

        }

    }
}

