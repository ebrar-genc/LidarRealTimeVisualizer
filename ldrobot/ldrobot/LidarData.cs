
using System;
using System.Data;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ldrobot
{
    class LidarData
    {
        #region Parameters

        private AppendToFile AppendToFile;

        /// <summary>
        /// List of angles
        /// </summary>
        private double[] Angles;

        /// <summary>
        /// distance and intensity information of the data
        /// </summary>
        private double[] Distance;
        private double[] Intensity;

        /// <summary>
        /// transmitted packet number.
        /// </summary>
        private int Packet;
        #endregion

        #region Public
        /// <summary>
        /// Initializes a new instance of the LidarData class.
        /// </summary>
        public LidarData()
        {
            Angles = new double[450 * 12];
            Distance = new double[450 * 12];
            Intensity = new double[450 * 12];

            AppendToFile = new AppendToFile();


            Packet = 0;
        }
        #endregion

        #region Private
        /// <summary>
        /// Clears the data arrays and resets the packet index.
        /// </summary>
        private void Clear()
        {
            Packet = 0;

        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Adds the provided lists of step angles and measurement points to the data arrays.
        /// </summary>
        /// <param name="Angles">List of step angles.</param>
        /// <param name="data">List of measurement points.</param>
        public void AddArray(double[] angles, double[] distance, double[] intensity)
        { 
            Angles = angles.Concat(Angles).ToArray();
            Distance = distance.Concat(Distance).ToArray();
            Intensity = intensity.Concat(Intensity).ToArray();

            Packet++;

            if (Packet == 1)
            {

                Debug.WriteLine(string.Join(", ", Angles));

                AppendToFile.AppendCorrectData(angles, distance, intensity);
                Clear();

            }
            
        }
        #endregion
        
    }
}
