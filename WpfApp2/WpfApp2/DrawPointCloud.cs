using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp2
{
    public class DrawPointCloud
    {
        private Canvas Canvas;

        public DrawPointCloud(Canvas canvas)
        {
            Canvas = canvas;
        }


        public void DrawPoints(double[] angles, double[] x, double[] y)
        {
            Canvas.Children.Clear();
            SolidColorBrush blackBrush = new SolidColorBrush();
            blackBrush.Color = Colors.Black;

            for (int i = 0; i < angles.Length; i++)
            {

                double angle = angles[i];
                double radius = 100;

                double ellipseLeft = Canvas.Width / 2 + x[i];
                double ellipseTop = Canvas.Height / 2 + y[i];
                Ellipse ellipse = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = blackBrush
                };

                Canvas.SetLeft(ellipse, ellipseLeft - ellipse.Width / 2);
                Canvas.SetTop(ellipse, ellipseTop - ellipse.Height / 2);
                Debug.WriteLine($"Elips Çizildi - X: {ellipseLeft}, Y: {ellipseTop}");

                Canvas.Children.Add(ellipse);

            }

        }


    }
}
