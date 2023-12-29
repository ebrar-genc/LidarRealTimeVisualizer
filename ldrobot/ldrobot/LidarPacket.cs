using static System.Runtime.InteropServices.JavaScript.JSType;

using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;
using System.Net.Sockets;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ldrobot;

/// <summary>
/// Represents a class for analyzing and parses the lidar data coming as a packet.
/// </summary>
public class LidarPacket
{
    #region Parameters

    private AppendToFile AppendToFile;
    private int StepCheck;
    private int PacketNumber;

    public List<float> Steps;
    public List<(string Name, ushort Value)> PacketValues;
    public List<(float Distance, float Intensity)> Data;// Measurement data (2 bytes distance (metre!!!), 1 byte intensity(metre!!!) per measurement point)

    /*public byte VerLen;// 3 bits packet type, 5 bits measurement points
    public ushort Speed; // degrees per second
    public float StartAngle; //radyan!!
    public float EndAngle;// radyan!!
    public ushort Timestamp;// millisecond
    public byte Crc;*/

    
    #endregion

    #region Public
    /// <summary>
    /// Initializes a new instance of the LidarPacket class. Sets initial values for data processing.
    /// </summary>
    public LidarPacket()
    {
        StepCheck = 0;
        PacketNumber = 0;

        PacketValues = new List<(string Name, ushort Value)> ();
        Data = new List<(float Distance, float Intensity)> ();
        Steps = new List<float> ();

        AppendToFile = new AppendToFile();

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

            ParseLidarPacket(buffer, measuringPoint);
            CalStepAngle(measuringPoint);

            AppendToFile.AppendToFilePacket(PacketValues);
            AppendToFile.AppendToFileSteps(Steps);
            AppendToFile.AppendToFileData(Data);

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
        float startAngle = (float)PacketValues[3].Value;
        float endAngle = (float)PacketValues[4].Value;

        float step = (endAngle - startAngle) / (measuringPoint - 1);

        for (int i = 0; i < measuringPoint; i++)
        {
            float angle = startAngle + (step * i);
            Steps.Add(angle);
            if (i == 0 && angle == startAngle
                || i == (measuringPoint - 1) && angle == endAngle)
                StepCheck++;
        }
        IsDataCheck();
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
        PacketValues.Add(("Header", buffer[0]));
        PacketValues.Add(("VerLen", buffer[1])); // 3 bits packet type, 5 bits measurement points
        PacketValues.Add(("Speed", (ushort)((buffer[3] << 8) | buffer[2]))); //degrees per second
        PacketValues.Add(("StartAngle", (ushort)((buffer[5] << 8) | buffer[4])));
        PacketValues.Add(("EndAngle", (ushort)((buffer[43] << 8) | buffer[42])));
        PacketValues.Add(("Timestamp", (ushort)((buffer[45] << 8) | buffer[44])));
        PacketValues.Add(("Crc", buffer[46])); // Crc 

        // Extract Data (2 bytes distance, 1 byte intensity per measurement point)
        for (int i = 0; i < measuringPoint * 3; i += 3)
        {
            float distance = (ushort)((buffer[i + 7] << 8) | buffer[i + 6]) / 1000.0f;
            float intensity = buffer[i + 8] / 1000.0f;
            Data.Add((distance, intensity));
        }
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
                Console.WriteLine("Incorrect Data!1111111111!!");
        }
    }
    #endregion

    #region Private
    /// <summary>
    /// Clears the internal state of the LidarPacket instance, preparing it for the next packet processing.
    /// </summary>
    private void Clear()
    {
        PacketValues.Clear();
        Data.Clear();
        Steps.Clear();
        
        if (PacketNumber == 450) 
        {
            //Console.WriteLine("10 packet was received");
            PacketNumber = 0;
            StepCheck = 0;
        }
    }
    #endregion
}
