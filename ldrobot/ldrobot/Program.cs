﻿class Program
{
    #region Main
    /// <summary>
    /// Main program to read LIDAR data. Initializes a LidarReader with specified parameters, starts reading LIDAR data,
    /// and stops the reading process when the Enter key is pressed. Handles exceptions that may occur during the reading.
    /// </summary>
    static void Main()
    {
        /// <summary>
        /// Create a new instance of LidarReader with the specified port and baud rate.
        /// </summary>
        LidarReader lidar = new LidarReader("COM5", 230400);

        try
        {
            Console.WriteLine("Reading LIDAR data. Press Enter to stop.");

            while (!Console.KeyAvailable)
            {
                lidar.StartReading();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while trying to read lidar data****" + ex.Message);
        }
        finally 
        { 
            lidar.StopReading();
            Console.WriteLine("LIDAR reading stopped. Press Enter to exit.");
            Console.ReadLine();
        }
    }
    #endregion
}