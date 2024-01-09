using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    /// <summary>
    /// Represents a class for appending LIDAR information to a file.
    /// </summary>
    class AppendToFile
    {
        #region Parameter
        private string ParserLidarData;
        private int Num;

        #endregion

        #region Public
        /// <summary>
        /// Initializes a new instance of the AppendToFile class.
        /// </summary>
        public AppendToFile()
        {
            ParserLidarData = "ParserLidarData.txt";
            Num = 1;

            if (File.Exists(ParserLidarData))
                File.Delete(ParserLidarData);
        }

        /// <summary>
        /// Appends correct Lidar information
        /// </summary>
        /// <param name="angles"> Each Lidar measurement points angles.</param>
        /// <param name="distance">Array of distances for each Lidar measurement point.</param>
        /// <param name="intensity">Array of intensities for each Lidar measurement point.</param>
        public void AppendLidarDatas(double[] angles, double[] distance, double[] x, double[] y)
        {
            int i = 1;

            using (StreamWriter sw = File.AppendText(ParserLidarData))
            {
                sw.WriteLine("****** " + Num + ". Data******");
                for (int j = 0; j < angles.Length; j++)
                {
                    sw.WriteLine(i + ". Angle: " + angles[j].ToString("F3") + " radian,     Distance: " + distance[j].ToString("F3") + " meter,     Intensity: ");
                    sw.WriteLine("X coordinate: " + x[j].ToString("F3") + ",       Y coordinate: " + y[j].ToString("F3") + "\n");
                    i++;
                }
                sw.WriteLine("\n\n");
            }
            Num++;
        }
        #endregion
    }
}
