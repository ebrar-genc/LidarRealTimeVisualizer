
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ldrobot
{
    class LidarData
    {
        #region Parameters
        /// <summary>
        /// object to write information to txt file
        /// </summary>
        private AppendToFile AppendToFile;

        /// <summary>
        /// Array of angles. radians.
        /// </summary>
        public double[] Angles;
        /// <summary>
        /// distance information of the lidar data
        /// </summary>
        public double[] Distance;
        /// <summary>
        /// intensity information of the lidar data
        /// </summary>
        public double[] Intensity;
        /// <summary>
        /// Number of packets required for 1 data
        /// </summary>
        private int PacketNum;
        /// <summary>
        /// Number of measuring points in 1 package
        /// </summary>
        private int MeasuringPoint;

        private double[] X;
        private double[] Y;
       // private double[] Z;

        #endregion

        #region Public
        /// <summary>
        /// Initializes a new instance of the LidarData class.
        /// </summary>
        public LidarData()
        {
            PacketNum = 38;
            MeasuringPoint = 12;

            Angles = new double[MeasuringPoint * PacketNum];
            Distance = new double[MeasuringPoint * PacketNum];
            Intensity = new double[MeasuringPoint * PacketNum];

            X = new double[MeasuringPoint * PacketNum];
            Y = new double[MeasuringPoint * PacketNum]; 
          //  Z = new double[MeasuringPoint * PacketNum];


            AppendToFile = new AppendToFile();


        }
        #endregion

        #region Private

        private void Clear()
        {
            Array.Clear(X, 0, X.Length);
            Array.Clear(Y, 0, Y.Length);

        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Sends lidar data to write to file
        /// </summary>
        public void SendData()
        {
            PolarToCartesian();
            AppendToFile.AppendCorrectDatas(Angles, Distance, Intensity, X, Y);

            Clear();
        }

        /// <summary>
        /// Converts polar coordinates to Cartesian coordinates.
        /// </summary>
        public void PolarToCartesian()
        {
            for (int i = 0; i < MeasuringPoint * PacketNum; i++)
            {
                X[i] = Distance[i] * Math.Cos(Angles[i]);
                Y[i] = Distance[i] * Math.Sin(Angles[i]);
            }

        }
        #endregion

    }
}
