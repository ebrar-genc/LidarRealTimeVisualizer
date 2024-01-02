
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
        private string FileName;
        #endregion

        #region Public
        /// <summary>
        /// Initializes a new instance of the AppendToFile class.
        /// </summary>
        public AppendToFile()
        {
            FileName = "lidarInfo.txt";
        }

        /// <summary>
        /// Appends the header information of a LIDAR packet to the file.
        /// </summary>
        /// <param name="packetValues">The list of header values to be appended.</param>
        public void AppendToFilePacket(List<(string name, ushort value)> packetValues)
        {
            using (StreamWriter sw = File.AppendText(FileName))
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
        public void AppendToFileSteps(List<float> steps)
        {
            int i = 1;

            using (StreamWriter sw = File.AppendText(FileName))
            {
                sw.WriteLine("\nStep angles:");
                foreach (var step in steps)
                {
                    sw.WriteLine("  " + i + ". step is: " + step);
                    i++;
                }
            }
        }

        /// <summary>
        /// Appends the 12 data points of a LIDAR packet to the file.
        /// </summary>
        /// <param name="data">The list of data points to be appended.</param>
        public void AppendToFileData(List<(float distance, float intensity)> data)
        {
            using (StreamWriter sw = File.AppendText(FileName))
            {
                sw.WriteLine("Lidar Data:");
                foreach (var dataPoint in data)
                {
                    sw.WriteLine("  Distance: " + dataPoint.distance + " metre, Intensity: " + dataPoint.intensity + " metre");
                }
                sw.WriteLine("\n\n");
            }
            
        }

        /// <summary>
        /// Appends the provided byte array to a file in hexadecimal format.
        /// </summary>
        /// <param name="buffer">The byte array representing package content.</param>
        public void AppendToFileBuffer(byte[] buffer)
        {
            using (StreamWriter sw = File.AppendText(FileName))
            {
                foreach (byte b in buffer)
                {
                    sw.Write(b.ToString("X2") + " ");
                }
                sw.WriteLine();
            }
        }
        #endregion
    }
}
