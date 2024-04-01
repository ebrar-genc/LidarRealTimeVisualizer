using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace WpfApp2
{
    public class DrawPointCloud
    {
        private Canvas Canvas;
        private double Centerx;
        private double Centery;
        private int EllipsesCount;


        public DrawPointCloud(Canvas lidarCanvas)
        {
            Canvas = lidarCanvas;
            Centerx = 400;
            Centery = 225;
            EllipsesCount = 456;
    }


    public void DrawPoints(double[] x, double[] y)
        {
            Canvas.Children.Clear();
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;

            for (int i = 0; i < EllipsesCount; i++)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = 5,
                    Height = 5,
                    Fill = blackBrush
                };
                Canvas.SetLeft(ellipse, Centerx - x[i]);
                Canvas.SetTop(ellipse, Centery - y[i]);

                Canvas.Children.Add(ellipse);

            }

        }


    }
}
