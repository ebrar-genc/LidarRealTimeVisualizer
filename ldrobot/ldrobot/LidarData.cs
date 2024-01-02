
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ldrobot
{
    class LidarData
    {
        #region Parameters
        /// <summary>
        /// List of angles
        /// </summary>
        public List<float>[] Angles;

        /// <summary>
        /// distance and intensity information of the data
        /// </summary>
        public List<(float Distance, float Intensity)>[] Data;

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
            Angles = new List<float>[450];
            Data = new List<(float Distance, float Intensity)>[450];

            Packet = 0;
        }
        #endregion

        #region Private
        /// <summary>
        /// Clears the data arrays and resets the packet index.
        /// </summary>
        private void Clear()
        {
            for (int i = 0; i < 450; i++)
            {
                Angles[i] = null;
                Data[i] = null;
            }

            Angles = new List<float>[450];
            Data = new List<(float Distance, float Intensity)>[450];
            Packet = 0;

        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Adds the provided lists of step angles and measurement points to the data arrays.
        /// </summary>
        /// <param name="Angles">List of step angles.</param>
        /// <param name="data">List of measurement points.</param>
        public void AddLists(List<float> angles, List<(float distance, float intensity)> data)
        {
            if (Angles[Packet] == null)
            {
                Angles[Packet] = new List<float>();
            }

            if (Data[Packet] == null)
            {
                Data[Packet] = new List<(float Distance, float Intensity)>();
            }
            Angles[Packet].AddRange(angles);
            Data[Packet].AddRange(data);

            Packet++;
            if (Packet == 449)
            {
                using (StreamWriter sw = new StreamWriter("correct.txt", true))
                {
                    sw.WriteLine("\ncorrect datas");
                    for (int i = 0; i < Data.Length; i++)
                    {
                        sw.WriteLine($"Packet {i + 1}:");

                        if (Data[i] != null)
                        {
                            foreach (var item in Data[i])
                            {
                                sw.WriteLine($"Distance: {item.Distance}, Intensity: {item.Intensity}");
                            }
                        }
                    }
                    Clear();

                }

            }
        }
        #endregion
        
    }
}
