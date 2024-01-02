
using Microsoft.VisualBasic;
using System.Xml.Linq;

namespace ldrobot
{
    /// <summary>
    /// Represents a class for appending LIDAR information to a file.
    /// </summary>
    class AppendToFile
    {
        #region Parameter

        /// <summary>
        /// file name where information will be saved
        /// </summary>
        private string LidarInfoFileName;
        private string LidarDataFileName;

        private int I;
        #endregion

        #region Public
        /// <summary>
        /// Initializes a new instance of the AppendToFile class.
        /// </summary>
        public AppendToFile()
        {
            LidarInfoFileName = "lidarInfo.txt";
            LidarDataFileName = "correctData.txt";

            I = 1;
        }

        /// <summary>
        /// Appends the header information of a LIDAR packet to the file.
        /// </summary>
        /// <param name="packetValues">The list of header values to be appended.</param>
        public void AppendToFilePacket(List<(string name, ushort value)> packetValues)
        {
            using (StreamWriter sw = File.AppendText(LidarInfoFileName))
            {

                foreach (var packet in packetValues)
                {
                    string line = (" - " + packet.name + ": " + packet.value + " - ");
                    sw.Write(line);
                }
            }
        }

        /// <summary>
        /// Appends the 12 step angles of a LIDAR packet to the file.
        /// </summary>
        /// <param name="steps">The list of step angles to be appended.</param>
        public void AppendToFileInfo(double[] angles, double[] distance, double[] intensity)
        {
            int i = 1;

            using (StreamWriter sw = File.AppendText(LidarInfoFileName))
            {
                for (int j = 0; j < angles.Length; j++)
                {
                    sw.WriteLine($"  {i}. Angle: {angles[j]}, Distance: {distance[j]}, Intensity: {intensity[j]}");
                    i++;
                }
            }
        }

        /// <summary>
        /// Appends the provided byte array to a file in hexadecimal format.
        /// </summary>
        /// <param name="buffer">The byte array representing package content.</param>
        public void AppendToFileBuffer(byte[] buffer)
        {
            using (StreamWriter sw = File.AppendText(LidarInfoFileName))
            {
                foreach (byte b in buffer)
                {
                    sw.Write(b.ToString("X2") + " ");
                }
                sw.WriteLine();
            }
        }

        public void AppendCorrectData(double[] angles, double[] distance, double[] intensity)
        {
            using (StreamWriter sw = new StreamWriter(LidarDataFileName, true))
            {
                for (int j = 0; j < angles.Length; j++)
                {
                    sw.WriteLine($"  {I}. Angle: {angles[j]}, Distance: {distance[j]}, Intensity: {intensity[j]}");
                    I++;
                }

            }
        }
        #endregion
    }
}
