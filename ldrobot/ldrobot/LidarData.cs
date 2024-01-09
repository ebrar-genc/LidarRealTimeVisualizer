
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
            Publisher = new LidarDataPublisher("tcp://localhost:3001");


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
        ///  Sends byte array to WP using publisher class
        /// </summary>
        public void SendData()
        {
            byte[] byteArray;

            PolarToCartesian();
            AppendToFile.AppendCorrectDatas(Angles, Distance, Intensity, X, Y);

            byteArray = DoubleArrayToByteArray();

            Publisher.SendMessage(byteArray);

            

        }

        /// <summary>
        /// It combines the created Cartesian coordinate values ​​and converts them into a byte array to send to WPF.
        /// </summary>
        /// <returns> value to send to wpf </returns>
        private byte[] DoubleArrayToByteArray()
        {
            int totalLength = X.Length + Y.Length + Angles.Length;
            double[] combined = new double[totalLength];

            X.CopyTo(combined, 0);
            Y.CopyTo(combined, X.Length);
            Angles.CopyTo(combined, X.Length + Y.Length);

            byte[] byteArray = new byte[totalLength * sizeof(double)];
            Buffer.BlockCopy(combined, 0, byteArray, 0, byteArray.Length);

            return byteArray;
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
