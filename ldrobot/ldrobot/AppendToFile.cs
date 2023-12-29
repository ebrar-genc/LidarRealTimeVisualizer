
namespace ldrobot
{
    class AppendToFile
    {
        private string FileName;

        public AppendToFile()
        {
            FileName = "lidarInfo.txt";
        }
        /// <summary>
        /// Saves the header information in one package to the file
        /// </summary>
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
        /// Saves the 12 step angles in one package to the file.
        /// </summary>
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
        /// Saves the 12 data points in one package to the file.
        /// </summary>
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
        /// <param name="buffer"> The byte array is the package content. </param>
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
    }
}
