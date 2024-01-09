using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp2
{
    class LidarDataParser
    {
        #region Parameters
        AppendToFile AppendToFile;
        /// <summary>
        /// Array of angles. radians.
        /// </summary>
        public double[] Angles;
        /// <summary>
        /// distance information of the lidar data
        /// </summary>
        public double[] Distance;
        public double[] X;
        public double[] Y;
        /// <summary>
        /// Number of packets required for 1 data
        /// </summary>
        private int PacketNum;
        /// <summary>
        /// Number of measuring points in 1 package
        /// </summary>
        private int MeasuringPoint;

        private int SegmentLength;
        #endregion

        #region Public
        public LidarDataParser()
        {
            AppendToFile = new AppendToFile();

            PacketNum = 38;
            MeasuringPoint = 12;
            SegmentLength = PacketNum * MeasuringPoint;

            Angles = new double[SegmentLength];
            Distance = new double[SegmentLength];
            X = new double[SegmentLength];
            Y = new double[SegmentLength];
        }
        #endregion

        #region Public Functions
        public Tuple<double[], double[], double[]> ParseData(byte[] byteArray)
        {
            ParseByteArray(byteArray);
            PolarToCartesian();
            AppendToFile.AppendLidarDatas(Angles, Distance, X, Y);
            var result = Tuple.Create(Angles, X, Y);
           // Clear();
            return result;
        }
        #endregion

        #region Private Functions

        public void ParseByteArray(byte[] byteArray)
        {
            Buffer.BlockCopy(byteArray, 0, Angles, 0, Angles.Length * sizeof(double));
            Buffer.BlockCopy(byteArray, Angles.Length * sizeof(double), Distance, 0, Distance.Length * sizeof(double));

        }
        /// <summary>
        /// Converts polar coordinates to Cartesian coordinates.
        /// </summary>
        private void PolarToCartesian()
        {
            for (int i = 0; i < SegmentLength; i++)
            {
                X[i] = Distance[i] * Math.Cos(Angles[i]);
                Y[i] = Distance[i] * Math.Sin(Angles[i]);
            }
        }

        private void Clear()
        {
            Array.Clear(Angles, 0, Angles.Length);
            Array.Clear(Distance, 0, Distance.Length);
            Array.Clear(X, 0, X.Length);
            Array.Clear(Y, 0, Y.Length);
            

        }
        #endregion
    }
}
