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
    /// Lidar data class for 38 accurate lidar packages, 450 measurement data
    /// </summary>
    private LidarData LidarData;
    /// <summary>
    /// time taken for each 38 packages
    /// </summary>
    private Stopwatch StopWatch;

    /// <summary>
    /// Number of measuring points in 1 package, (12pieces)
    /// </summary>
    private int MeasuringPoint;
    /// <summary>
    /// Checking for star and end angle values ​​of 1 package
    /// </summary>
    private bool StepCheck;
    /// <summary>
    /// A boolean value indicating whether the CRC is successful.
    /// </summary>
    private bool CrcCheck;
    /// <summary>
    /// Number of packets transmitted
    /// </summary>
    private int PacketNumber;

    private List<List<(string Name, ushort Value)>> PacketValues;

    private byte[][] Buffer;

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
        PacketNumber = -1;
        CrcCheck = true;
        StepCheck = true;

        StopWatch.Start();
        Debug.WriteLine("StopWatch is started!");

        PacketValues = new List<List<(string Name, ushort Value)>>();
        Buffer = new byte[38][];

    }
    #endregion

    #region Private
    /// <summary>
    /// Clears the internal state of the LidarPacket instance, preparing it for the next packet processing.
    /// </summary>
    private void Clear()
    {
        PacketNumber = -1;
        CrcCheck = true;
        StepCheck = true;

        Array.Clear(LidarData.Angles, 0, LidarData.Angles.Length);
        Array.Clear(LidarData.Distance, 0, LidarData.Distance.Length);
        Array.Clear(LidarData.Intensity, 0, LidarData.Intensity.Length);
        PacketValues.Clear();
        Array.Clear(Buffer, 0, Buffer.Length);
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
        ParseLidarPacket(buffer);
        StepCheck = CalStepAngle();

        if (!CrcCheck || !StepCheck)
        {
            PacketNumber = 37;
        }

        if (PacketNumber == 37)
        {
            IsLidarData();
            Clear();
            StopWatch.Restart();
        }

    }

    /// <summary>
    /// The Start angle of each package is increased by 12 steps to determine the final angle.
    /// If the angles match, the packet was transmitted correctly.
    /// </summary>
    /// <param name="measuringPoint"> The number of measurement points in a data.(12) </param>
    public bool CalStepAngle()
    {
        int stepNum;
        double startAngle = (double)PacketValues[PacketNumber][3].Value;
        double endAngle = (double)PacketValues[PacketNumber][4].Value;

        stepNum = 0;
        double step = (endAngle - startAngle) / (MeasuringPoint - 1);

        for (int i = 0; i < MeasuringPoint; i++)
        {
            double angle = startAngle + (step * i);
            LidarData.Angles[PacketNumber * MeasuringPoint + i] = (angle / 100) * Math.PI / 180;
            if (i == 0 && angle == startAngle
                || i == (MeasuringPoint - 1) && angle == endAngle)
                stepNum++;
        }
        if (stepNum == 2)
            return true;
        else
        {
            Debug.WriteLine(PacketNumber + ". packet have wrong step number");
            return false;
        }
    }
    #endregion

    #region Private Functions
    /// <summary>
    /// It is parsed in accordance with the package format.
    /// 
    /// </summary>
    /// <param name="buffer"> 1 packet byte array </param>
    private void ParseLidarPacket(byte[] buffer)
    {
        int j = 0;
        PacketValues.Add(new List<(string Name, ushort Value)>());
        PacketValues[PacketNumber].Add(("Header (1byte)", buffer[0]));
        PacketValues[PacketNumber].Add(("VerLen (1byte)", buffer[1])); // 3 bits packet type, 5 bits measurement points
        PacketValues[PacketNumber].Add(("Speed (LSB MSB)", (ushort)((buffer[3] << 8) | buffer[2]))); //degrees per second
        PacketValues[PacketNumber].Add(("StartAngle (LSB MSB)", (ushort)((buffer[5] << 8) | buffer[4])));
        PacketValues[PacketNumber].Add(("EndAngle (LSB MSB)", (ushort)((buffer[43] << 8) | buffer[42])));
        PacketValues[PacketNumber].Add(("Timestamp (LSB MSB)", (ushort)((buffer[45] << 8) | buffer[44])));
        PacketValues[PacketNumber].Add(("Crc (1byte)", buffer[46])); // Crc

        // Extract Data (2 bytes distance, 1 byte intensity per measurement point)
        for (int i = 0; i < MeasuringPoint * 3; i += 3)
        {
            double distance = (ushort)((buffer[i + 7] << 8) | buffer[i + 6]) / 1000.0f;
            double intensity = buffer[i + 8] / 1000.0f;
            LidarData.Distance[PacketNumber * MeasuringPoint + j] = distance;
            LidarData.Intensity[PacketNumber * MeasuringPoint + j] = intensity;
            j++;
        }
        Buffer[PacketNumber] = buffer;

    }
    private void IsLidarData()
    {
        if (CrcCheck && StepCheck)
        {
            Debug.WriteLine("Correct Data!!");
            Console.WriteLine("Seconds of delivery of 38 packages: " + StopWatch.Elapsed.TotalSeconds);
            StopWatch.Restart();

            AppendToFile.AppendPackets(PacketValues, Buffer);
            LidarData.SendData();
        }
        else
            Debug.WriteLine("Incorrect Data!!");
       
    }
    #endregion
}
