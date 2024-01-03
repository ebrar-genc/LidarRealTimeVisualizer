
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

            AppendToFile = new AppendToFile();


        }
        #endregion

        #region Private

        private void Clear()
        {

        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Sends lidar data to write to file
        /// </summary>
         public void SendData()
        {
            AppendToFile.AppendCorrectDatas(Angles, Distance, Intensity);
        }
        #endregion
        
    }
}
