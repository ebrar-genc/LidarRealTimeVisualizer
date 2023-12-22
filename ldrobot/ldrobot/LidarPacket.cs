using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LidarPacket
{
    public byte Header { get; set; } // 0x54
    public byte VerLen { get; set; } // 3 bits packet type, 5 bits measurement points
    public ushort Speed { get; set; } // degrees per second
    public ushort StartAngle { get; set; } // 0.01 degrees
    public byte[] Data { get; set; } // Measurement data (3 bytes per measurement point)
    public ushort EndAngle { get; set; } // 0.01 degrees
    public ushort Timestamp { get; set; } // milliseconds
    public byte CRC { get; set; }

    public LidarPacket()
    {
        // Initialize Data array with a default length (assuming a specific number of measurement points)
        Data = new byte[3 * GetMeasurementPointCount()];
    }
    private int GetMeasurementPointCount()
    {
        // Extract the lower 5 bits from VerLen to determine the number of measurement points
        return VerLen & 0x1F;
        // 0x1F = 00011111
        // masking
    }

    public static LidarPacket FromByteArray(byte[] byteArray)
    {
        if (byteArray == null || byteArray.Length < 14) // Minimum size check!!!!!
            throw new ArgumentException("Invalid byte array for LidarPacket");

        LidarPacket lidarPacket = new LidarPacket();

        lidarPacket.Header = byteArray[0];
        lidarPacket.VerLen = byteArray[1];
        lidarPacket.Speed = BitConverter.ToUInt16(byteArray, 2);
        lidarPacket.StartAngle = BitConverter.ToUInt16(byteArray, 4);

        // Extract measurement data
        int measurementPointCount = lidarPacket.GetMeasurementPointCount();
        int dataOffset = 6; // Offset after speed and start angle
        lidarPacket.Data = byteArray.Skip(dataOffset).Take(3 * measurementPointCount).ToArray();

        // Continue extracting other fields based on their sizes
        int endAngleIndex = dataOffset + 3 * measurementPointCount;
        lidarPacket.EndAngle = BitConverter.ToUInt16(byteArray, endAngleIndex);

        int timestampIndex = endAngleIndex + 2;
        lidarPacket.Timestamp = BitConverter.ToUInt16(byteArray, timestampIndex);

        int crcIndex = timestampIndex + 2;
        lidarPacket.CRC = byteArray[crcIndex];

        return lidarPacket;
    }

    public override string ToString()
    {
        return $"Header: {Header}, VerLen: {VerLen}, Speed: {Speed}, StartAngle: {StartAngle}, " +
               $"EndAngle: {EndAngle}, Timestamp: {Timestamp}, CRC: {CRC}";
    }
}

