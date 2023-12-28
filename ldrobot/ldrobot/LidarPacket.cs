using ldrobot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

public class LidarPacket
{
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
    public LidarPacket()
    {
        StepCheck = 0;
        PacketNumber = 0;

        Data = new List<(float Distance, float Intensity)>();
        Step = new List<float>();
    }

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

    private void ParseLidarPacket(byte[] buffer, int measuringPoint)
    {
        Speed = (ushort)((buffer[3] << 8) | buffer[2]);
        StartAngle = (ushort)((buffer[5] << 8) | buffer[4]) * 0.01f * (float)Math.PI / 180.0f;

        // Extract Data (2 bytes distance, 1 byte intensity per measurement point)
        for (int i = 0; i < measuringPoint * 3; i += 3)
        {
            float distance = (ushort)((buffer[i + 7] << 8) | buffer[i + 6]) / 1000.0f;
            float intensity = buffer[i + 8] / 1000.0f;
            Data.Add((distance, intensity));
        }

        EndAngle = (ushort)((buffer[43] << 8) | buffer[42]) * 0.01f * (float)Math.PI / 180.0f;
        Timestamp = (ushort)((buffer[45] << 8) | buffer[44]);
    }


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

    private void IsDataCheck()
    {
        if (PacketNumber == 10)
        {
            if (StepCheck == 20)
                Console.WriteLine("Correct Data");
            else
                Console.WriteLine("Incorrect Data!1111111111!!");

        }
    }

    public void AppendToFilePacket()
    {
        using (StreamWriter sw = File.AppendText("lidarPacket.txt"))
        {
            string result = ("Header (byte): " + Header.ToString("X2") + " VerLen (byte): " + VerLen.ToString("X2") +
                " Speed (ushort): " + Speed + "\nTimestamp(ushort): " + Timestamp + " Crc (byte): " + Crc.ToString("X2") +
                "\nStartAngle (radyan): " + StartAngle + " EndAngle (radyan): " + EndAngle);

            sw.WriteLine(result);
            sw.WriteLine("12 measuring point step control angles:");
            foreach (float step in Step)
            {
                sw.WriteLine(step);
            }
            sw.WriteLine("Lidar Data:");
            foreach (var dataPoint in Data)
            {
                sw.WriteLine("  Distance: " + dataPoint.Distance + " metre, Intensity: " + dataPoint.Intensity + " metre");
            }

            sw.WriteLine();
        }
    }

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
}
