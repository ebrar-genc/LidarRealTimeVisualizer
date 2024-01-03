
using Microsoft.VisualBasic;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ldrobot
{
    /// <summary>
    /// Represents a class for appending LIDAR information to a file.
    /// </summary>
    class AppendToFile
    {
        #region Parameter

        /// <summary>
        /// The file in which lidar information will be saved
        /// </summary>
        private string LidarInfoFileName;
        /// <summary>
        /// file where correct lidar information is saved
        /// </summary>
        private string LidarCoorrectDataFileName;
        /// <summary>
        /// number of info data transmitted
        /// </summary>
        private int InfoNum;
        /// <summary>
        /// number of correct data transmitted
        /// </summary>
        private int CorrectNum;

        #endregion

        #region Public
        /// <summary>
        /// Initializes a new instance of the AppendToFile class.
        /// </summary>
        public AppendToFile()
        {
            LidarInfoFileName = "lidarInfo.txt";
            LidarCoorrectDataFileName = "correctData.txt";

            InfoNum = 1;
            CorrectNum = 1;

            if (File.Exists(LidarInfoFileName))
                File.Delete(LidarInfoFileName);
            if (File.Exists(LidarCoorrectDataFileName))
                File.Delete(LidarCoorrectDataFileName);
        }

        /// <summary>
        /// Appends Lidar packet information.
        /// </summary>
        /// <param name="packetValues">List of packet values, each representing Lidar packet details.</param>
        /// <param name="buffer">Array of byte arrays representing the packet header byte content for each Lidar packet.</param>
        public void AppendPackets(List<List<(string name, ushort value)>> packetValues, byte[][] buffer)
        {
            using (StreamWriter sw = File.AppendText(LidarInfoFileName))
            {
                sw.WriteLine("****** " + InfoNum + ". Data******");
                for (int i = 0; i < buffer.Length; i++)
                {
                    sw.WriteLine((i + 1) + ". Packet\nBuffer: " + BitConverter.ToString(buffer[i]));

                    sw.WriteLine("PacketValues:");
                    foreach (var packet in packetValues[i])
                    {
                        sw.WriteLine(" - " + packet.name + ":" + packet.value);
                    }
                }
                sw.WriteLine("\n\n");
                InfoNum++;
            }
        }

        /// <summary>
        /// Appends correct Lidar information
        /// </summary>
        /// <param name="angles"> Each Lidar measurement points angles.</param>
        /// <param name="distance">Array of distances for each Lidar measurement point.</param>
        /// <param name="intensity">Array of intensities for each Lidar measurement point.</param>
        public void AppendCorrectDatas(double[] angles, double[] distance, double[] intensity)
        {
            int i = 1;

            using (StreamWriter sw = File.AppendText(LidarCoorrectDataFileName))
            {
                sw.WriteLine("****** " + CorrectNum + ". Data******");
                for (int j = 0; j < angles.Length; j++)
                {
                    sw.WriteLine(i + ". Angle: " + angles[j] + ",       Distance: " + distance[j] + ",       Intensity: " + intensity[j]);
                    i++;
                }
                sw.WriteLine("\n\n");
            }
            CorrectNum++;
        }
        #endregion
    }
}
