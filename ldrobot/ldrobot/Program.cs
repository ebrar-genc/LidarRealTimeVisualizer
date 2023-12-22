using System.IO.Ports;
class Program
{
    static void Main()
    {
        LidarData lidar = new LidarData("COM5", 230400); //get paramtre

        lidar.StartReading();
        Console.WriteLine("Reading LIDAR data. Press Enter to stop.");
        Console.ReadLine();

        lidar.StopReading();

        Console.WriteLine("LIDAR reading stopped. Press Enter to exit.");
        Console.ReadLine();
    }
}