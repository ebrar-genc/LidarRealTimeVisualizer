using static System.Runtime.InteropServices.JavaScript.JSType;

using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text.RegularExpressions;

/// <summary>
/// Represents a class for analyzing and parses the lidar data coming as a packet.
/// </summary>
public class LidarPacket
{
    #region Parameters
    private byte Header; // 0x54
    private byte VerLen;// 3 bits packet type, 5 bits measurement points
    private ushort Speed; // degrees per second
    private float StartAngle; //radyan!!
    private List<(float Distance, float Intensity)> Data;// Measurement data (2 bytes distance (metre!!!), 1 byte intensity(metre!!!) per measurement point)
    private float EndAngle;// radyan!!
    private ushort Timestamp;// millisecond
    private byte Crc;

    private List<float> Step;
    private int StepCheck;
    private int PacketNumber;
    #endregion

    #region Public
    /// <summary>
    /// Initializes a new instance of the LidarPacket class. Sets initial values for data processing.
    /// </summary>
    public LidarPacket()
    {
        StepCheck = 0;
        PacketNumber = 0;

        Data = new List<(float Distance, float Intensity)>();
        Step = new List<float>();
    }
    #endregion

    #region Public Functions

    /// <summary>
    /// Analyzes the provided LIDAR packet, extracts information, calculates step angles,
    /// performs data checks, and appends packet details to a file.
    /// </summary>
    /// <param name="buffer">The LIDAR packet.</param>
    /// <param name="measuringPoint">The number of measurement points in a data.(12) </param>
    public void AnalyzeLidarPacket(byte[] buffer, int measuringPoint)
    {
        if (buffer != null && buffer.Length == 47)
        {
            PacketNumber++;

            Header = buffer[0];
            VerLen = buffer[1];
            Crc = buffer[46];

            ParseLidarPacket(buffer, measuringPoint);
            CalStepAngle(measuringPoint);
            AppendToFilePacket();
            Clear();
        }
    }

    /// <summary>
    /// The Start angle of each package is increased by 12 steps to determine the final angle.
    /// If the angles match, the packet was transmitted correctly.
    /// </summary>
    /// <param name="measuringPoint"> The number of measurement points in a data.(12) </param>
    public void CalStepAngle(int measuringPoint)
    {
        float step = (EndAngle - StartAngle) / (measuringPoint - 1);

        for (int i = 0; i < measuringPoint; i++)
        {
            float angle = StartAngle + (step * i);
            Step.Add(angle);
            if (i == 0 && Math.Round(angle, 5) == Math.Round(StartAngle, 5)
                || i == (measuringPoint - 1) && Math.Round(angle, 5) == Math.Round(EndAngle, 5))
                StepCheck++;
        }
        IsDataCheck();
    }

    /// <summary>
    /// Saves the header information, 12 measurement points and 12 step angles of the data in one package to the file.
    /// </summary>
    public void AppendToFilePacket()
    {
        using (StreamWriter sw = File.AppendText("lidarPacket.txt"))
        {
            string result = ("Header (byte): " + Header.ToString("X2") + " VerLen (byte): " + VerLen.ToString("X2") +
                " Speed (ushort): " + Speed + "\nTimestamp(ushort): " + Timestamp + " Crc (byte): " + Crc.ToString("X2") +
                "\nStartAngle (radyan): " + StartAngle + " EndAngle (radyan): " + EndAngle);

            sw.WriteLine("*****");
            sw.WriteLine(result);
            sw.WriteLine("\n12 measuring point step control angles:");
            foreach (float step in Step)
            {
                sw.WriteLine(step);
            }
            sw.WriteLine("\nLidar Data:");
            foreach (var dataPoint in Data)
            {
                sw.WriteLine("  Distance: " + dataPoint.Distance + " metre, Intensity: " + dataPoint.Intensity + " metre");
            }

            sw.WriteLine();
        }
    }
    #endregion

    #region Private Functions

    /// <summary>
    /// It is parsed in accordance with the package format.
    /// </summary>
    /// <param name="buffer"> 1 packet byte array </param>
    /// <param name="measuringPoint"> The number of measurement points in a data.(12) </param>
    private void ParseLidarPacket(byte[] buffer, int measuringPoint)
    {
        Speed = (ushort)((buffer[3] << 8) | buffer[2]);
        StartAngle = (ushort)((buffer[5] << 8) | buffer[4]) * 0.01f;

        // Extract Data (2 bytes distance, 1 byte intensity per measurement point)
        for (int i = 0; i < measuringPoint * 3; i += 3)
        {
            float distance = (ushort)((buffer[i + 7] << 8) | buffer[i + 6]) / 1000.0f;
            float intensity = buffer[i + 8] / 1000.0f;
            Data.Add((distance, intensity));
        }

        EndAngle = (ushort)((buffer[43] << 8) | buffer[42]) * 0.01f;
        Timestamp = (ushort)((buffer[45] << 8) | buffer[44]);
    }

    /// <summary>
    /// 450 packages are needed to obtain 1 piece of data correctly.
    /// If 1 packet was transmitted correctly, StepCheck is increased by 2
    /// </summary>
    private void IsDataCheck()
    {
        if (PacketNumber == 450)
        {
            if (StepCheck == 450 * 2)
                Console.WriteLine("Correct Data");// correctdata class(()))!!
            else
                Debug.WriteLine("Incorrect Data!1111111111!!");
        }
    }
    #endregion

    #region Private
    /// <summary>
    /// Clears the internal state of the LidarPacket instance, preparing it for the next packet processing.
    /// </summary>
    private void Clear()
    {
        Header = 0;
        VerLen = 0;
        Speed = 0;
        StartAngle = 0;
        Data.Clear();
        Step.Clear();
        EndAngle = 0;
        Timestamp = 0;
        Crc = 0;
        if (PacketNumber == 10) 
        {
            //Console.WriteLine("10 packet was received");
            PacketNumber = 0;
            StepCheck = 0;
        }
    }
    #endregion
}
