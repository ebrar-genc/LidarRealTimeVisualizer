
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ldrobot
{
    public class LidarData
    {
        #region Parameters
        /// <summary>
        /// object to write information to txt file
        /// </summary>
        private AppendToFile AppendToFile;

        /// <summary>
        /// Sends data to WPF using netMQ
        /// </summary>
        private LidarDataPublisher Publisher;

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


            AppendToFile = new AppendToFile();
            Publisher = new LidarDataPublisher("tcp://localhost:3001");


        }
        #endregion

        #region Private

    
        #endregion

        #region Public Functions
        /// <summary>
        /// Sends lidar data to write to file
        ///  Sends byte array to WP using publisher class
        /// </summary>
        public void SendData()
        {
            byte[] byteArray;

            AppendToFile.AppendCorrectDatas(Angles, Distance, Intensity);

            byteArray = DoubleArrayToByteArray();

            Publisher.SendMessage(byteArray);

            Clear();

        }

        /// <summary>
        /// It combines the angles and didstance values ​​and converts them into a byte array to send to WPF.
        /// </summary>
        /// <returns> value to send to wpf </returns>
        private byte[] DoubleArrayToByteArray()
        {
            int totalLength = Angles.Length + Distance.Length;
            double[] doubleCombined = new double[totalLength];

            Angles.CopyTo(doubleCombined, 0);
            Distance.CopyTo(doubleCombined, Angles.Length);
            byte[] byteArray = new byte[totalLength * sizeof(double)];
            Buffer.BlockCopy(doubleCombined, 0, byteArray, 0, byteArray.Length);

            return byteArray;
        }

        private void Clear()
        {
            Array.Clear(Angles, 0, Angles.Length);
            Array.Clear(Distance, 0, Distance.Length);
            Array.Clear(Intensity, 0, Intensity.Length);



        }
        #endregion

    }
}
