using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LidarPacket
{
    public byte Header { get; set; } // 0x54
    public byte VerLen { get; set; } // 3 bits packet type, 5 bits measurement points
    public ushort Speed { get; set; } // degrees per second
    public ushort StartAngle { get; set; } //degree
    public List<(ushort Distance, byte Intensity)> Data { get; set; } // (mm) Measurement data (2 bytes distance, 1 byte intensity per measurement point)
    public ushort EndAngle { get; set; } // degree
    public ushort Timestamp { get; set; } // millisecond
    public byte CrcCheck { get; set; }


    public override string ToString()
    {
        string result = $"Header (byte): {Header}, VerLen (byte): {VerLen}, Speed (ushort): {Speed}, StartAngle (ushort): {StartAngle}, EndAngle (ushort): {EndAngle}, Timestamp (ushort): {Timestamp}, CrcCheck (byte): {CrcCheck}";
        
        Debug.Write($"Lidar Data:");
        foreach (var dataPoint in Data)
        {
            Debug.Write($"  Distance: {dataPoint.Distance} mm, Intensity: {dataPoint.Intensity}");
        }
        Clear();
        return result;
    }

    private void Clear()
    {
        Header = 0;
        VerLen = 0;
        Speed = 0;
        StartAngle = 0;
        Data = null;
        EndAngle = 0;
        Timestamp = 0;
        CrcCheck = 0;
    }
}

