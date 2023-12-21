/// <summary>
/// Represents the data structure for LIDAR data.
/// </summary>
public class LidarData
{
    #region Properties

    /// <summary>
    /// Gets or sets the header of the LIDAR data.
    /// </summary>
    public byte Header { get; private set; }

    /// <summary>
    /// Gets or sets the version and length information of the LIDAR data.
    /// </summary>
    public byte VersionLength { get; private set; }

    /// <summary>
    /// Gets or sets the speed information of the LIDAR data in degrees per second.
    /// </summary>
    public ushort Speed { get; private set; }

    /// <summary>
    /// Gets or sets the start angle of the LIDAR data in degrees.
    /// </summary>
    public ushort StartAngle { get; private set; }

    /// <summary>
    /// Gets or sets the data payload of the LIDAR data.
    /// </summary>
    public byte[] Data { get; private set; }

    /// <summary>
    /// Gets or sets the end angle of the LIDAR data in degrees.
    /// </summary>
    public ushort EndAngle { get; private set; }

    /// <summary>
    /// Gets or sets the timestamp of the LIDAR data in milliseconds.
    /// </summary>
    public ushort Timestamp { get; private set; }

    /// <summary>
    /// Gets or sets the CRC (Cyclic Redundancy Check) of the LIDAR data.
    /// </summary>
    public byte CRC { get; private set; }

    #endregion Properties

    #region Public Functions

    /// <summary>
    /// Creates a new instance of the LidarData class from raw byte data.
    /// </summary>
    /// <param name="rawData">The raw byte data representing LIDAR data.</param>
    /// <returns>A LidarData object populated with parsed data.</returns>
    public static LidarData FromRawData(byte[] rawData)
    {
        LidarData lidarData = new LidarData();

        lidarData.Header = rawData[0];
        lidarData.VersionLength = rawData[1];
        lidarData.Speed = BitConverter.ToUInt16(rawData, 2);
        lidarData.StartAngle = BitConverter.ToUInt16(rawData, 4);

        int dataLength = lidarData.VersionLength & 0x1F;
        lidarData.Data = new byte[dataLength * 3];
        Array.Copy(rawData, 6, lidarData.Data, 0, lidarData.Data.Length);

        int dataIndex = 6 + lidarData.Data.Length;
        lidarData.EndAngle = BitConverter.ToUInt16(rawData, dataIndex);
        lidarData.Timestamp = BitConverter.ToUInt16(rawData, dataIndex + 2);
        lidarData.CRC = rawData[dataIndex + 4];

        return lidarData;
    }

    /// <summary>
    /// Returns a string representation of the LidarData object.
    /// </summary>
    /// <returns>A string containing formatted information about the LidarData object.</returns>
    public override string ToString()
    {
        return $"Header: 0x{Header:X}\n" +
               $"VersionLength: 0x{VersionLength:X}\n" +
               $"Speed: {Speed} degrees per second\n" +
               $"Start Angle: {StartAngle * 0.01} degrees\n" +
               $"Data Length: {Data.Length} bytes\n" +
               $"End Angle: {EndAngle * 0.01} degrees\n" +
               $"Timestamp: {Timestamp} milliseconds\n" +
               $"CRC: 0x{CRC:X}";
    }
    #endregion
}