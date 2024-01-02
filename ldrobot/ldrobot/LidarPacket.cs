using System;
using System.Diagnostics;
using ldrobot;

/// <summary>
/// Represents a class for analyzing and parses the lidar data coming as a packet.
/// </summary>
public class LidarPacket
{
    #region Parameters
    /// <summary>
    /// Class for writing lidar information to txt
    /// </summary>
    private AppendToFile AppendToFile;
    /// <summary>
    /// crc check for accuracy of transmitted packet
    /// </summary>
    private LidarCrcCheck LidarCrcCheck;
    /// <summary>
    /// Lidar data class for 450 accurate lidar packages
    /// </summary>
    private LidarData LidarData;
    /// <summary>
    /// time taken for each 450 packages
    /// </summary>
    private Stopwatch StopWatch;

    /// <summary>
    /// Number of measuring points in 1 package
    /// </summary>
    private int MeasuringPoint;
    /// <summary>
    /// Checking for star and end angle values ​​of 1 package
    /// </summary>
    private int StepCheck;

    /// <summary>
    /// A boolean value indicating whether the CRC is successful.
    /// </summary>
    private bool CrcCheck;

    /// <summary>
    /// Number of packets transmitted
    /// </summary>
    private int PacketNumber;

    public List<float> Steps;
    public List<(string Name, ushort Value)> PacketValues;
    public List<(float Distance, float Intensity)> Data;// Measurement data (2 bytes distance (metre!!!), 1 byte intensity(metre!!!) per measurement point)

    #endregion

    #region Public
    /// <summary>
    /// Initializes a new instance of the LidarPacket class. Sets initial values for data processing.
    /// </summary>
    public LidarPacket()
    {
        AppendToFile = new AppendToFile();
        LidarCrcCheck = new LidarCrcCheck();
        LidarData = new LidarData();
        StopWatch = new Stopwatch();

        MeasuringPoint = 12;
        StepCheck = 0;
        PacketNumber = 0;
        CrcCheck = true;

        StopWatch.Start();

        PacketValues = new List<(string Name, ushort Value)> ();
        Data = new List<(float Distance, float Intensity)> ();
        Steps = new List<float> ();
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
            PacketNumber = 0;
            StepCheck = 0;
        }
    }
    #endregion

    #region Public Functions
    /// <summary>
    /// Analyzes the provided LIDAR packet, extracts information, calculates step angles,
    /// performs data checks, and appends packet details to a file.
    /// </summary>
    /// <param name="buffer">The LIDAR packet.</param>
    public void AnalyzeLidarPacket(byte[] buffer)
    {
        PacketNumber++;
        CrcCheck = LidarCrcCheck.CalculateCrc8(buffer, buffer.Length - 1);

        if (CrcCheck)
        {
            ParseLidarPacket(buffer);
            CalStepAngle();
            LidarData.AddLists(Steps, Data);

            AppendToFile.AppendToFilePacket(PacketValues);
            AppendToFile.AppendToFileSteps(Steps);
            AppendToFile.AppendToFileData(Data);
        }
        else
            Debug.WriteLine("CRC error!");
        if (PacketNumber == 450)
        {
            IsLidarData();
        }
        Clear();
    }

    /// <summary>
    /// The Start angle of each package is increased by 12 steps to determine the final angle.
    /// If the angles match, the packet was transmitted correctly.
    /// </summary>
    /// <param name="measuringPoint"> The number of measurement points in a data.(12) </param>
    public void CalStepAngle()
    {
        float startAngle = (float)PacketValues[3].Value;
        float endAngle = (float)PacketValues[4].Value;

        float step = (endAngle - startAngle) / (MeasuringPoint - 1);

        for (int i = 0; i < MeasuringPoint; i++)
        {
            float angle = startAngle + (step * i);
            Steps.Add(angle);
            if (i == 0 && angle == startAngle
                || i == (MeasuringPoint - 1) && angle == endAngle)
                StepCheck++;
        }
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// It is parsed in accordance with the package format.
    /// </summary>
    /// <param name="buffer"> 1 packet byte array </param>
    private void ParseLidarPacket(byte[] buffer)
    {
        PacketValues.Add(("Header", buffer[0]));
        PacketValues.Add(("VerLen", buffer[1])); // 3 bits packet type, 5 bits measurement points
        PacketValues.Add(("Speed", (ushort)((buffer[3] << 8) | buffer[2]))); //degrees per second
        PacketValues.Add(("StartAngle", (ushort)((buffer[5] << 8) | buffer[4])));
        PacketValues.Add(("EndAngle", (ushort)((buffer[43] << 8) | buffer[42])));
        PacketValues.Add(("Timestamp", (ushort)((buffer[45] << 8) | buffer[44])));
        PacketValues.Add(("Crc", buffer[46])); // Crc 

        // Extract Data (2 bytes distance, 1 byte intensity per measurement point)
        for (int i = 0; i < MeasuringPoint * 3; i += 3)
        {
            float distance = (ushort)((buffer[i + 7] << 8) | buffer[i + 6]) / 1000.0f;
            float intensity = buffer[i + 8] / 1000.0f;
            Data.Add((distance, intensity));
        }
    }
    private void IsLidarData()
    {
        if (StepCheck == 450 * 2)
        {
            Debug.WriteLine("Correct Data");
            Console.WriteLine("Seconds of delivery of 450 packages: " + StopWatch.Elapsed.TotalSeconds);
            StopWatch.Restart();
        }
        else
        {
            Debug.WriteLine("Incorrect Data!!! 450 packages were received. The number of matching steps should have been 900, but: " + StepCheck);
        }
    }
    #endregion
}
