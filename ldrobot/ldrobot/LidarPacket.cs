using ldrobot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LidarPacket
{
    public byte Header { get; set; } // 0x54
    public byte VerLen { get; set; } // 3 bits packet type, 5 bits measurement points
    public ushort Speed { get; set; } // degrees per second
    public ushort StartAngle { get; set; } //degree
    public List<(ushort Distance, ushort Intensity)> Data { get; set; } // (mm) Measurement data (2 bytes distance, 1 byte intensity per measurement point)
    public ushort EndAngle { get; set; } // degree
    public ushort Timestamp { get; set; } // millisecond
    public byte Crc { get; set; }

    public void ParseLidarData(byte[] rawData, int measuringPoint)
    {
        if (rawData != null && rawData.Length == 47)
        {
            Header = rawData[0];
            VerLen = rawData[1];

            Speed = (ushort)((rawData[3] << 8) | rawData[2]);

            StartAngle = (ushort)((rawData[5] << 8) | rawData[4]);

            // Extract Data (2 bytes distance, 1 byte intensity per measurement point)
            Data = new List<(ushort Distance, ushort Intensity)>();
            for (int i = 0; i < measuringPoint * 3; i += 3)
            {
                ushort distance = (ushort)((rawData[i + 7] << 8) | rawData[i + 6]);
                ushort intensity = rawData[i + 8];
                Data.Add((distance, intensity));
            }

            EndAngle = (ushort)((rawData[43] << 8) | rawData[42]);
            Timestamp = (ushort)((rawData[45] << 8) | rawData[44]);
            Crc = (rawData[46]);
        }
    }
    public void CalculateAngles(int measuringPoint)
    {
        float startAngle = StartAngle;
        float endAngle = EndAngle;

        float step = (endAngle - startAngle) / (measuringPoint - 1);

        for (int i = 0; i < measuringPoint; i++)
        {
            float angle = startAngle + step * i;
            Console.WriteLine($"Measurement Point {i + 1}: Angle = {angle}");
        }
    }

    public void AppendToFilePacket()
    {
        using (StreamWriter sw = File.AppendText("lidarPacket.txt"))
        {
            string result = ("Header (byte): " + Header.ToString("X2") + " VerLen (byte): " + VerLen.ToString("X2") +
                " Speed degrees per second (byte): " + Speed.ToString("X2") + " StartAngle (ushort): " + StartAngle +
                " EndAngle (ushort): " + EndAngle + " Timestamp (ushort): " + Timestamp + " Crc (byte): " + Crc.ToString("X2"));

            sw.WriteLine(result);
            sw.WriteLine("Lidar Data:");
            foreach (var dataPoint in Data)
            {
                sw.WriteLine($"  Distance: {dataPoint.Distance} mm, Intensity: {dataPoint.Intensity}");
            }
        }
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
        Crc = 0;
    }
}

