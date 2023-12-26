using System.IO.Ports;
class Program
{
    static void Main()
    {
        LidarReader lidar = new LidarReader("COM5", 230400); //get paramters

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
            Console.WriteLine("An error occurred while trying to read lidar data" + ex.Message);
        }
        finally 
        { 
            lidar.StopReading();
            Console.WriteLine("LIDAR reading stopped. Press Enter to exit.");
            Console.ReadLine();
        }
    }
}